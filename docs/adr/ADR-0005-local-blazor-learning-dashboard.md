# ADR-0005: Use a local Blazor learning dashboard for realtime observability

- Status: Accepted
- Date: 2026-07-11

## Context

Uma API e um hub isolados deixam o encadeamento persistência → projeção → push menos tangível em um laboratório de entrevistas.

## Decision

Incluir uma UI Blazor local que chama a API e conecta ao hub reais. Ela exibe o roteiro, payloads, fontes de atualização e falhas esperadas.

## Consequences

Ela não é uma UI pública, não possui requisitos de produto nem duplica regras de negócio. É uma ferramenta de observabilidade e aprendizado, limitada ao localhost.
