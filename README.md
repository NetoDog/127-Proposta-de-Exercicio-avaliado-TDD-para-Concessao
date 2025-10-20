# Gamification Domain (TDD exercise)

Projeto minimal para o exercício "TDD para Concessão de Badges por Missão".

## Como rodar
1. Tenha o .NET 9 SDK instalado.
2. No diretório raiz do projeto execute:
   ```bash
   dotnet test
   ```

## O que está aqui
- `src/Gamification.Domain` - domínio com serviços, políticas, modelos e portas (interfaces).
- `tests/Gamification.Domain.Tests` - suíte de testes xUnit cobrindo:
  - Concessão básica
  - Janelas de bônus (integral / reduzido / nenhum)
  - Elegibilidade
  - Idempotência básica via requestId and chave natural simulated
  - Falhas de configuração

## Observações
- Persistência real não é implementada; usei fakes nos testes.
- A `IAwardsWriteStore.SalvarConcessaoAtomica` representa operação atômica (se lançar exceção, nada deve ser parcialmente gravado).
- Decisões de design e limitações estão no enunciado original (entregue pelo instrutor).

