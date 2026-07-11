# 004 - Caso de uso e contracts de projeção

## Mudança

Adiciona o caso de uso de aplicação, contracts de persistência/publicação e a projeção de leitura com timeline. A publicação ocorre apenas depois da gravação retornar com sucesso.

## Rastreabilidade ADR

- ADR aplicada: ADR-0002 e ADR-0003.

## Testes

O teste de aplicação foi introduzido em vermelho e verifica a ordem observável `persist → publish`.
