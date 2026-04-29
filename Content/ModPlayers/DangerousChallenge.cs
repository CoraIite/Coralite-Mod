using System;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public event Action OnChallengeFailed;

        public int ChallengeHitCount { get; private set; } = -1;

        public void StartChallenge(int hitLimit,Action onFail)
        {
            ChallengeHitCount = hitLimit;
            OnChallengeFailed += onFail;
        }

        public void ChallengeHit()
        {
            if (ChallengeHitCount>0)
            {
                ChallengeHitCount--;
                if (ChallengeHitCount < 1)
                {
                    OnChallengeFailed?.Invoke();
                    OnChallengeFailed = null;
                }
            }
        }

        public void ResetChallenge()
        {
            ChallengeHitCount = -1;
            OnChallengeFailed = null;
        }
    }
}
