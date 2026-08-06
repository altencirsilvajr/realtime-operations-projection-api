# 014 - Add self-contained local start scripts

## Commit

`chore: add self-contained local start/stop scripts`

## Objetivo

Permitir subir e encerrar o laboratorio localmente com `./scripts/start-local.sh` e
`./scripts/stop-local.sh`, sem depender de um helper compartilhado fora do repositorio.

## Implementacao

- versiona `scripts/start-local.sh`, `scripts/stop-local.sh` e `scripts/project-local.sh`;
- o helper fica dentro do proprio repo e espera a URL responder antes de declarar pronto;
- ignora `.local-pids/` e `.local-logs/` gerados em runtime.

## Rastreabilidade ADR

Decisao local sem ADR novo: scripts de bootstrap local nao mudam a arquitetura do laboratorio;
apenas tornam o fluxo de estudo reproduzivel apos o repositorio ser clonado.

## Verificacao

- `bash -n scripts/start-local.sh scripts/stop-local.sh scripts/project-local.sh`
- smoke local em labs sem Docker e no cqrs-order-read-model-api: start aguarda readiness e HTTP responde.

## Alternativas e trade-offs

- Manter helper compartilhado em pasta pai: quebrou quando os repos passaram a viver em `dotnet-sr/`.
- Script autocontido por repo: mais duplicacao, mas cada clone funciona sozinho.

## Proximo passo

Rodar `./scripts/start-local.sh` apos o clone e estudar o fluxo do laboratorio.
