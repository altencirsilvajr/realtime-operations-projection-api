# 009 - Corrigir bindings de comandos no dashboard

## Mudança

Substitui expressões Razor com aspas aninhadas por handlers nomeados para as transições do painel.

## Rastreabilidade ADR

- ADR aplicada: ADR-0005; mantém o painel executando comandos reais pela API.

## Verificação

`dotnet build src/RealtimeOperationsProjection.Dashboard/RealtimeOperationsProjection.Dashboard.csproj --no-restore --tl:off` passou sem avisos.
