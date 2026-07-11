# ADR-0004: Make reconnect recover from a persisted snapshot

- Status: Accepted
- Date: 2026-07-11

## Context

SignalR é um canal de notificação, não um log confiável para clientes desconectados. Mensagens podem ser perdidas durante uma reconexão.

## Decision

Ao reconectar com uma operação selecionada, o cliente Blazor busca `GET /api/operations/{id}`. O push contém a projeção atual, mas não é a única fonte de verdade.

## Consequences

O painel indica se o estado veio de `snapshot` ou `push`; a correção não depende de replay do hub.
