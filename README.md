# Realtime Operations Projection API

Laboratório .NET 9 para entrevistas Senior: uma operação muda de estado, registra evento, atualiza uma projeção persistida e só então notifica observadores via SignalR.

## Objetivo

Demonstrar a separação entre comando, regra de domínio, projeção operacional e canal realtime. O dashboard Blazor é uma ferramenta local de estudo, não uma interface de produto.

## Arquitetura

```text
Blazor dashboard ─HTTP─> Minimal API ─> OperationService ─> SQLite
       ^                    |               |                ├─ aggregate
       └── SignalR push ────┴───────────────┴────────────────┼─ domain event
                 only after persisted projection              └─ read projection
```

- `Domain`: máquina de estados e eventos.
- `Application`: caso de uso e contracts (`IOperationStore`, `IOperationProjectionPublisher`).
- `Infrastructure`: SQLite; agregado, eventos e projeção são atualizados juntos.
- `Api`: endpoints Minimal, ProblemDetails, health check e um hub sem regra de negócio.
- `Dashboard`: chama a API e o hub reais; em reconexão, relê o snapshot persistido.

## Como rodar

Pré-requisito: SDK .NET 9.

```powershell
dotnet run --project .\src\RealtimeOperationsProjection.Api
dotnet run --project .\src\RealtimeOperationsProjection.Dashboard
```

- API: http://localhost:5308
- Health: http://localhost:5308/health
- UI local: http://localhost:5408

SQLite é embutido; portanto não há dependência externa ou Docker Compose a subir. Para produção/múltiplas instâncias, a publicação SignalR deve migrar para Redis/Azure SignalR e a gravação/publicação deve usar outbox.

## Como testar

```powershell
dotnet test .\RealtimeOperationsProjection.sln
```

## Fluxo visual

```text
POST create → Created projection persisted → SignalR push
POST Processing → event + projection persisted → SignalR push
disconnect/reconnect → GET current persisted projection (snapshot recovery)
```

## Guia de estudo

Siga primeiro `POST /api/operations`, depois a transição para `Processing`; no painel, compare o card **State source** e o payload trace. Leia ADR-0002, ADR-0003 e ADR-0004 nessa ordem. Perguntas de entrevista respondidas: onde vive a regra de negócio? por que a projeção existe? por que push não é fonte de verdade? como o desenho muda em scale-out?

## Decisões Que Eu Consigo Defender Em Entrevista

- Hub é borda de comunicação; regras vivem no domínio/aplicação (ADR-0002).
- Projeção reduz acoplamento da leitura ao agregado e é persistida antes do push (ADR-0003).
- Reconexão recarrega snapshot: mensagens perdidas não corrompem a tela (ADR-0004).
- SQLite mantém o laboratório simples; scale-out pede backplane e outbox.
- A UI é uma sonda local de observabilidade, não ampliação de escopo de produto (ADR-0005).
