# Reserved compute gate (schema 149)

When `reservedComputeGate` is enabled, `SignReservedCompute` needs lab reputation at least `minReputationPermille` and respects a per-lab cooldown (`reservedSignCooldownUntil`). Successful signs emit `ReservedComputeSigned` (Q474). Closes the bible tail on vendor reputation in [compute contracts](compute-contracts.md).