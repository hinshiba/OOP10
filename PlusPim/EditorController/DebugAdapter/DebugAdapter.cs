using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;

namespace PlusPim.EditorController.DebugAdapter;

internal class DebugAdapter: DebugAdapterBase {
    internal DebugAdapter(Stream input, Stream output) {
        this.InitializeProtocolClient(input, output);
        this.Protocol.Run();
    }

    protected override InitializeResponse HandleInitializeRequest(InitializeArguments arguments) {
        // InitializeRequestに対してResponseを返す前は，イベントを送信してはならない
        // 返さないといけないレスポンスに，戻り値の型が設定されているので便利
        return new InitializeResponse();
    }

    protected override LaunchResponse HandleLaunchRequest(LaunchArguments args) {

        this.Protocol.SendEvent(new OutputEvent {
            Output = "Handler: LaunchRequest.\n",
            Category = OutputEvent.CategoryValue.Console
        });

        // 即座にterminatedイベントを送信
        //this.Protocol.SendEvent(new TerminatedEvent());

        return new LaunchResponse();
    }

    protected override DisconnectResponse HandleDisconnectRequest(DisconnectArguments args) {

        this.Protocol.SendEvent(new OutputEvent {
            Output = "Handler: DisconnectRequest.\n",
            Category = OutputEvent.CategoryValue.Console
        });

        return new DisconnectResponse();
    }

}
