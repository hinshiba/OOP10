namespace PlusPim.Runtime.Services;

internal class RuntimeService(IProcessor processor, IMemory memory): IRuntimeService {
    public void ExecuteSteps(uint stepNum) {
        for(uint i = 0; i < stepNum; i++) {
            processor.Execute();
        }

        throw new NotImplementedException();
        // todo リザルト的なものを返す
    }

    public void LoadProgram(string path) {
        // todo ここでファイルを読み込んでバイナリか命令のenum列にしてmemoryにセットする
        throw new NotImplementedException();

    }

    public void Reset() {
        memory.Reset();
        processor.Reset();

        throw new NotImplementedException();
    }
}

