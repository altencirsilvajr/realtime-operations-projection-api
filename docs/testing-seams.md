# Testing seams

Os testes são escritos contra estas bordas públicas:

- Domínio: `Operation.Register` e `Operation.TransitionTo` preservam a máquina de estados e expõem eventos de domínio.
- Aplicação: `OperationService` coordena persistência, projeção e publicação nesta ordem.
- HTTP: a API Minimal expõe contracts e ProblemDetails via `WebApplicationFactory`.
- Navegador: o painel local exercita API e SignalR reais.
