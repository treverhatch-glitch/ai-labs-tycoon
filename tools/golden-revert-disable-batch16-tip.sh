# shellcheck shell=bash
disable_batch16_at_tip() {
  for _b16 in marginCall hearingSharePrice rungSixCrisis scenarioGoals lawTarget; do disable_cfg "$_b16"; done
  comment_hook 'RungSixCrisis.RunWeek'
  comment_hook 'ScenarioGoals.RunWeek'
  sed -i 's|MarginCall.RunWeek(c, state, lab);||' Rules/Systems/Finance.cs
  sed -i '/HearingSharePrice.OnAnswered/d' Rules/Systems/OfficeEvents.cs
  sed -i '/RungSixCrisis.OnArcAnswered/d' Rules/Systems/OfficeEvents.cs
}