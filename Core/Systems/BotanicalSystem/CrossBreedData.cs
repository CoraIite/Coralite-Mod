
//namespace Coralite.Core.Systems.BotanicalSystem
//{
//    public struct CrossBreedData
//    {
//        /// <summary>
//        /// 父植物类型
//        /// </summary>
//        public readonly int FatherPlantType;
//        /// <summary>
//        /// 母植物类型
//        /// </summary>
//        public readonly int MotherPlantType;
//        /// <summary>
//        /// 突变植物种类
//        /// </summary>
//        public readonly int MutantPlantType;
//        /// <summary>
//        /// 突变概率，举例 1的话就是1%的概率
//        /// </summary>
//        public readonly int Percentage;

//        public CrossBreedData(int fatherPlantType, int motherPlantType, int mutantPlantType, int percentage)
//        {
//            FatherPlantType = fatherPlantType;
//            MotherPlantType = motherPlantType;
//            MutantPlantType = mutantPlantType;
//            Percentage = percentage;
//        }

//        public bool CanMutant(int fatherType, int motherType)
//        {
//            if (fatherType == FatherPlantType && motherType == MotherPlantType)
//                return true;
//            return false;
//        }

//        public static bool operator ==(CrossBreedData aData, CrossBreedData bData)
//        {
//            return aData.Equals(bData);
//        }

//        public static bool operator !=(CrossBreedData aData, CrossBreedData bData)
//        {
//            return !aData.Equals(bData);
//        }

//        public override bool Equals(object obj)
//        {
//            if (obj is null)
//                return false;

//            return obj is CrossBreedData && Equals((CrossBreedData)obj);
//        }

//        public bool Equals(CrossBreedData other)
//        {
//            if (FatherPlantType == other.FatherPlantType && MotherPlantType == other.MotherPlantType &&
//                MutantPlantType == other.MutantPlantType && Percentage == other.Percentage)
//                return true;

//            return false;
//        }

//        public override int GetHashCode() => (((((FatherPlantType * 397) ^ MotherPlantType) * 397) ^ MutantPlantType) * 397) ^ Percentage;

//    }

//    public enum CrossBreedState : byte
//    {
//        notfound = 0,
//        error = 1,
//        success = 2,
//        failure = 3,
//    }
//}
