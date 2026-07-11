# 008 - Corrigir leitura da timeline SQLite

## Mudança

Evita `ORDER BY DateTimeOffset` no SQL SQLite, que o provider não traduz. A timeline pequena do laboratório é ordenada em memória depois da leitura.

## Rastreabilidade ADR

- ADR aplicada: ADR-0003, pois a correção mantém a leitura pela projeção persistida.

## Testes

O teste de integração `WebApplicationFactory` capturou o 500 e passa após a correção ao reler a projeção criada.
