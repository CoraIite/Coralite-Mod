using Coralite.Systems.CoraliteBehaviorTree.Enums;

namespace Coralite.Systems.CoraliteBehaviorTree.Interfaces
{
    public interface IBehaviour
    {
        //
        //  //创建对象请调用Create()释放对象请调用Release()
        //  protected Behavior() {
        //    setStatus(EStatus.Invalid);
        //  }

        EStatus Tick();//设置调用顺序，onInitialize--update--onTerminate

        void OnInitialize();//当节点调用前

        EStatus Update();//节点操作的具体实现

        void OnTerminate(EStatus Status); //节点调用后执行

        void Release();//释放对象所占资源

        void AddChild(IBehaviour child);

        void SetStatus(EStatus status);

        EStatus GetStatus();

        void Reset();

        void Abort();

    }
}
