using AiLabs.Rules.Sim;

namespace AiLabs.Unity.Hq
{
  public static class HqOperationsHud
  {
    public static string RunningCostLine(Simulation sim) => "";
    public static string ModelStripLine(LabState lab) => lab?.Models?.Count.ToString() ?? "0";
  }
}
