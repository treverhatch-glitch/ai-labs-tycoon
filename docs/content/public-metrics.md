# Public metrics

Customer-visible uptime board per model (bible ch. 6; `publicMetrics.*`; Q456). Model fields `pubOn`, `pubScore`, `pubMiss` (schema 134). Only the player's lab.

`PublishMetrics` and `StopMetrics` turn the board on or off. Each week the score tracks how much demand was served; incidents mark misses and move trust, and weak scores push conformity like public evals. Events: `MetricsPublished`, `MetricsStopped`.