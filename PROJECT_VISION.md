# Realtime Operations Projection API

Um laboratório vertical, compacto e estudável, para demonstrar como propagar mudanças operacionais quase em tempo real sem concentrar regras de negócio em um hub SignalR.

O fluxo essencial registra uma operação, transiciona seu status, grava um evento de domínio, persiste uma projeção de leitura e só então notifica clientes conectados. O painel Blazor local serve exclusivamente para observar esse fluxo usando chamadas reais da API.

## Critérios de sucesso

- A regra de transição pertence ao domínio/aplicação, nunca ao hub.
- A leitura operacional é servida por uma projeção persistida, separada do agregado.
- Cada push SignalR acontece após a persistência da projeção.
- Um cliente reconectado recupera o estado por snapshot HTTP.
- O repositório é pequeno o bastante para estudo e denso o bastante para entrevistas Senior.
