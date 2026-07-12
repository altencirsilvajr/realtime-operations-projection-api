# 013 - Corrigir contrato de enums no SignalR

## Mudança

Alinha o serializador JSON do SignalR ao contrato HTTP, emitindo `OperationStatus` como texto. Antes, o hub emitia números e o dashboard não conseguia desserializar o payload cujo status é textual.

## Rastreabilidade ADR

- ADR aplicada: ADR-0002, ADR-0003 e ADR-0005.

## Verificação

O cenário com painel observador conectado deve exibir `SignalR push` após um comando disparado em outro painel.
