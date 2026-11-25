# shellcheck shell=bash
# Disable batch-fifteen at tip when golden-reverting an earlier batch (sourced after disable_cfg/comment_hook exist).
disable_batch15_at_tip() {
  for _b15 in campaignDeadline lawShaping takeoffCrisis scenarioBaggage; do disable_cfg "$_b15"; done
  comment_hook 'CampaignDeadline.RunWeek'
  comment_hook 'TakeoffCrisis.RunWeek'
  sed -i 's|b.Loans = ExitDebt.LoanBalance(lab)|b.Loans = 0|' Rules/Systems/DisplayQueries.cs
  sed -i 's|b.Notes = ExitDebt.NoteBalance(lab)|b.Notes = 0|' Rules/Systems/DisplayQueries.cs
  sed -i 's|b.Rbf = ExitDebt.RbfBalance(lab)|b.Rbf = 0|' Rules/Systems/DisplayQueries.cs
  sed -i 's|b.CovenantCalls = ExitDebt.CovenantCalls(c, lab)|b.CovenantCalls = 0|' Rules/Systems/DisplayQueries.cs
  sed -i 's|b.Total = gross - b.StaffShare - b.Loans - b.Notes - b.Rbf - b.CovenantCalls;|b.Total = gross - b.StaffShare;|' Rules/Systems/DisplayQueries.cs
  sed -i '/lab.Id == state.PlayerLabId)/,+1d' Rules/Systems/EraRules.cs
}