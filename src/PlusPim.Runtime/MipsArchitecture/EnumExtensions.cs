using System.ComponentModel;
using System.Reflection;

namespace PlusPim.Runtime.MipsArchitecture;

/// <summary>
/// Enum型のDescriptionAttributeを扱うための拡張メソッド
/// </summary>
internal static class EnumExtensions {
    /// <summary>
    /// EnumのDescriptionAttributeの値を取得する
    /// </summary>
    public static string GetDescription(this Enum value) {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if(field == null) {
            return value.ToString();
        }

        DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Description属性から対応するEnum値を取得する
    /// </summary>
    public static bool TryParseByDescription<TEnum>(string description, out TEnum result) where TEnum : struct, Enum {
        foreach(TEnum value in Enum.GetValues<TEnum>()) {
            if(((Enum)(object)value).GetDescription().Equals(description, StringComparison.OrdinalIgnoreCase)) {
                result = value;
                return true;
            }
        }
        result = default;
        return false;
    }
}
