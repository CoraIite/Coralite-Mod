namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public interface IFairyPowder
    {
        void EditFairyAttempt(ref FairyAttempt attempt);
        void OnCostPowder(Fairy fairy, FairyAttempt attempt,FairyCatcherProj catcherProj);
    }
}
