# 002 - Máquina de estados do domínio

## Mudança

Cria o agregado `Operation`, seus status e o evento de domínio emitido depois de uma transição válida.

## Rastreabilidade ADR

- ADR aplicada: ADR-0002, para manter a regra de transição fora do hub.

## Testes

O teste do seam de domínio foi escrito em vermelho antes da implementação e agora verifica a transição `Created → Processing` e o evento resultante.
