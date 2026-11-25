# Traffic router

Split serving demand across released models (bible ch. 6; `trafficRouter.*`; Q455). Model field `routePpm` (schema 133). Only the player's lab.

`SetRouteShare` assigns ppm per model; the total across released models cannot exceed one million. Skewed routes plus low served permille add incident risk. Events: `RouteShareSet`.