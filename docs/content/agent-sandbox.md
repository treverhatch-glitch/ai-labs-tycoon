# Agent sandbox

Per-model isolation before agents get code and data tools (bible ch. 6; `agentSandbox.*`; Q452). Model fields `sandLeft` and `sandGrad` (schema 130). Only the player's lab.

`StartSandbox` costs cash, needs safety staff, and runs for several weeks. Until graduation, tool policy is web-only; after a pass, `sandGrad` is 1 and code or data tools are allowed. Events: `SandboxStarted`, `SandboxFinished` (Pick 1 passed, 0 failed).