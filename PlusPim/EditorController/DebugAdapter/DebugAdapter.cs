using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Newtonsoft.Json.Linq;
using PlusPim.Application;

namespace PlusPim.EditorController.DebugAdapter;

internal class DebugAdapter: DebugAdapterBase {
    private readonly IApplication _app;

    internal DebugAdapter(Stream input, Stream output, IApplication app) {
        this._app = app;
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

        if(args.ConfigurationProperties.TryGetValue("program", out JToken program)) {
            // エラーハンドリングはtodo
            _ = this._app.Load(program.Value<string>());
        } else {
            this.Protocol.SendEvent(new OutputEvent {
                Output = "program field not found in JSON\n",
                Category = OutputEvent.CategoryValue.Console
            });
            // プログラムが指定されていない場合はterminatedイベントを送信
            this.Protocol.SendEvent(new TerminatedEvent());
        }


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
