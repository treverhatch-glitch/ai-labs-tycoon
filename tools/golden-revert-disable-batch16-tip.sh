# shellcheck shell=bash
# Disable batch-sixteen at tip when golden-reverting an earlier batch (sourced after disable_cfg/comment_hook exist).
disable_batch16_at_tip() {
  for _b16 in marginCall hearingSharePrice rungSixCrisis scenarioGoals lawTarget; do disable_cfg "$_b16"; done
  comment_hook 'RungSixCrisis.RunWeek'
  comment_hook 'ScenarioGoals.RunWeek'
  sed -i 's|MarginCall.RunWeek(c, state, lab);||' Rules/Systems/Finance.cs
  sed -i 's|^[[:space:]]*HearingSharePrice.OnAnswered|            // HearingSharePrice.OnAnswered|' Rules/Systems/OfficeEvents.cs
  sed -i 's|^[[:space:]]*if (e.Trigger == OfficeTrigger.Escape|            // if (e.Trigger == OfficeTrigger.Escape|' Rules/Systems/OfficeEvents.cs
  sed -i 's|^[[:space:]]*RungSixCrisis.OnArcAnswered|                // RungSixCrisis.OnArcAnswered|' Rules/Systems/OfficeEvents.cs
}