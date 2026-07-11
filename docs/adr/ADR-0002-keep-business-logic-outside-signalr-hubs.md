# ADR-0002: Keep business logic outside SignalR hubs

- Status: Accepted
- Date: 2026-07-11

## Context

Hubs são bordas de transporte e possuem ciclo de vida de conexão; aplicar transições neles torna a regra difícil de testar e acopla o domínio ao realtime.

## Decision

Endpoints HTTP chamam um caso de uso de aplicação. O hub `OperationsHub` apenas expõe a conexão e recebe notificações do adaptador de publicação.

## Consequences

As regras podem ser testadas sem SignalR. Para escalar múltiplas instâncias, o adaptador de publicação pode usar Redis/Azure SignalR sem mover a regra.
