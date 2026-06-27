namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 连招（包壳法）：连招仍走旧 <c>switch(Combo)</c>，但 <see cref="Combo"/> 改为本地步进，<br/>
    /// 由已同步的外层状态 ID（<c>ai[0]</c>）约束；子招内部的生成已全部服务端守卫。<br/>
    /// 各方法返回 <see langword="true"/> 表示整段连招结束，由包壳态的服务端逻辑推进选招。
    /// </summary>
    public partial class ZacurrentDragon
    {
        public bool RunNormalRoarCombo1()
        {
            switch (Combo)
            {
                default:
                case 0:
                    if (Roar())
                    {
                        ResetFields();
                        ElectricBallSetStartValue();
                        Combo = 1;
                    }
                    break;
                case 1:
                    if (ElectricBall())
                    {
                        ResetFields();
                        Combo = 2;
                    }
                    break;
                case 2:
                    if (GravitationThunder())
                    {
                        ResetFields();
                        Combo = 3;
                    }
                    break;
                case 3:
                    if (ElectromagneticCannon())
                    {
                        ResetFields();
                        Combo = 4;
                    }
                    break;
                case 4:
                    if (GatherCurrent())
                    {
                        ResetFields();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool RunNormalRoarCombo2()
        {
            switch (Combo)
            {
                default:
                case 0:
                    if (Roar())
                    {
                        ResetFields();
                        Combo = 1;
                    }
                    break;
                case 1:
                    if (ElectricChain(100))
                    {
                        ResetFields();
                        Combo = 2;
                        LightningRaidSetStartValue();
                        Recorder2 = 3;//必定进行3次长冲
                    }
                    break;
                case 2:
                    if (LightningRaidNoraml())
                    {
                        ResetFields();
                        Combo = 3;
                    }
                    break;
                case 3:
                    if (GatherCurrent())
                    {
                        ResetFields();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool RunNormalChainCombo()
        {
            switch (Combo)
            {
                default:
                case 0:
                    if (ElectricChain(60))
                    {
                        ResetFields();
                        Combo = 1;
                    }
                    break;
                case 1:
                    if (FallingThunder())
                    {
                        ResetFields();
                        Combo = 2;
                    }
                    break;
                case 2:
                    if (DashDischarging())
                    {
                        ResetFields();
                        Combo = 3;
                    }
                    break;
                case 3:
                    if (GatherCurrent())
                    {
                        ResetFields();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool RunNormalPointerCombo()
        {
            switch (Combo)
            {
                default:
                case 0:
                    if (AimThunderBall(90))
                    {
                        ResetFields();
                        Combo = 1;
                    }
                    break;
                case 1:
                    if (ElectricChain(300))
                    {
                        ResetFields();
                        Combo = 2;
                    }
                    break;
                case 2:
                    if (ElectricBreathMiddle())
                    {
                        ResetFields();
                        Combo = 3;
                    }
                    break;
                case 3:
                    if (FallingThunder())
                    {
                        ResetFields();
                        Combo = 4;
                    }
                    break;
                case 4:
                    if (GatherCurrent())
                    {
                        ResetFields();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool RunVoltBigCombo()
        {
            switch (Combo)
            {
                default:
                case 0:
                    if (Roar())
                    {
                        ResetFields();
                        Combo = 1;
                    }
                    break;
                case 1:
                    if (ElectricChain(10))
                    {
                        ResetFields();
                        Combo = 2;
                    }
                    break;
                case 2:
                case 4:
                case 6:
                    if (PointerBallP2(120))
                    {
                        ResetFields(false);
                        LightningRaidSetStartValue();
                        Recorder2 = 1;
                        Combo++;
                    }
                    break;
                case 3:
                case 5:
                case 7:
                    if (LightningRaidVolt())
                    {
                        ResetFields(false);
                        Combo++;
                    }
                    break;
                case 8:
                    if (GravitationThunder(60 * 6))
                    {
                        ResetFields(false);
                        Combo++;
                    }
                    break;
                case 9:
                    if (ElectricBreathMiddle(2))
                    {
                        ResetFields(false);
                        Combo++;
                        ZThunderBallSetStartValue();
                    }
                    break;
                case 10:
                    if (ZThunderBall())
                    {
                        ResetFields(false);
                        VoltBreakSetStartValue();
                        Combo++;
                    }
                    break;
                case 11:
                    if (VoltBreak())
                    {
                        ResetFields(false);
                        Combo++;
                    }
                    break;
                case 12:
                    if (FallingThunder())
                    {
                        ResetFields();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool RunVoltChainCombo()
        {
            switch (Combo)
            {
                default:
                case 0:
                    if (ElectricChain(140))
                    {
                        ResetFields(false);
                        VoltBreakSetStartValue();
                        Combo++;
                    }
                    break;
                case 1:
                    if (VoltBreak())
                    {
                        ResetFields(false);
                        Combo++;
                    }
                    break;
                case 2:
                    if (ElectricBreathMiddle())
                    {
                        ResetFields();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool RunVoltZBallChainCombo()
        {
            switch (Combo)
            {
                default:
                case 0:
                    if (ZThunderBall())
                    {
                        ResetFields(false);
                        LightningRaidSetStartValue();
                        Recorder2 = 3;
                        Combo++;
                    }
                    break;
                case 1:
                    if (LightningRaidVolt())
                    {
                        ResetFields(false);
                        VoltBreakSetStartValue();
                        Combo++;
                    }
                    break;
                case 2:
                    if (VoltBreak())
                    {
                        ResetFields(false);
                        Combo++;
                    }
                    break;
                case 3:
                    if (PointerBallP2(180))
                    {
                        ResetFields(false);
                        Combo++;
                    }
                    break;
                case 4:
                    if (FallingThunder())
                    {
                        ResetFields();
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}
