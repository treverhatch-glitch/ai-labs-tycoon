# shellcheck shell=bash
disable_batch22_at_tip() {
  for _b22 in lawTargetEraAmplify specialistBranchEraDiscount marginCallEraGrace; do
    disable_cfg "$_b22"
  done
  if [ -f "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch23-tip.sh" ]; then
    # shellcheck source=golden-revert-disable-batch23-tip.sh
    source "$(dirname "${BASH_SOURCE[0]}")/golden-revert-disable-batch23-tip.sh"
    disable_batch23_at_tip
  fi
}