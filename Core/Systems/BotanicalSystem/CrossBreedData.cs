
namespace Coralite.Core.Systems.BotanicalSystem
{
	public struct CrossBreedData
	{
		/// <summary>
		/// 突变植物种类
		/// </summary>
		public readonly int MutantPlantType;
		/// <summary>
		/// 突变概率，举例 100的话就是1%的概率
		/// </summary>
		public readonly int Percentage;

		public CrossBreedData(int mutantPlantType,int percentage)
		{
			MutantPlantType = mutantPlantType;
			Percentage = percentage;
		}
	}

    public enum CrossBreedState : byte
    {
        notfound = 0,
		error=1,
        success = 2,
        failure = 3,
    }
}
