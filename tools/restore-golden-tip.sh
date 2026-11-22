#!/bin/bash
# Reset golden-revert touch files to the committed tip (after batch-13 revert tests).
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
  Rules/Systems/MarketSystem.cs Rules/Systems/ToolPolicy.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.ToolPolicy.cs \
  Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  git checkout HEAD -- "$f"
done