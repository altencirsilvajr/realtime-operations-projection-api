# ADR-0003: Use projections for operational status reads

- Status: Accepted
- Date: 2026-07-11

## Context

O agregado preserva invariantes de escrita; a tela operacional precisa de uma visão direta, cronológica e estável para leitura.

## Decision

Persistir `OperationProjection` como modelo de leitura separado. O caso de uso atualiza a projeção na mesma transação que o agregado e o evento, depois publica a atualização.

## Consequences

Leituras não dependem da forma interna do agregado. O laboratório usa SQLite local; em escala, a projeção poderia ser atualizada assincronamente via outbox/eventos, aceitando consistência eventual.
