# 005 - API, SQLite, projeção e SignalR

## Mudança

Implementa persistência SQLite de agregado, projeção e eventos em uma gravação; expõe Minimal API, health check, logs estruturados e hub de notificação. O hub não contém regra de negócio.

## Rastreabilidade ADR

- ADR aplicada: ADR-0002 e ADR-0003.
- Decisão local que não exige ADR: SQLite reduz setup de um laboratório local; uma implementação de scale-out pode trocar o publicador por Redis/Azure SignalR e introduzir outbox.

## Testes

O teste HTTP foi criado em vermelho com `WebApplicationFactory` antes da implementação do host público e do endpoint.
