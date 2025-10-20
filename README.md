🏅 Gamification Domain — TDD para Concessão de Badges por Missão
🎯 Objetivo

Este projeto implementa, via Test-Driven Development (TDD), a regra de domínio de concessão de badges (insígnias) para estudantes que concluem missões dentro de um contexto de gamificação acadêmica (ClassHero).

A implementação cobre unicidade, elegibilidade, janelas de bônus, idempotência, atomicidade e auditoria, conforme descrito na proposta de exercício.

⚙️ Tecnologias Utilizadas

C# 12

.NET 9

xUnit (framework de testes)

Arquitetura Hexagonal (Ports and Adapters)

📌 Nenhuma dependência de persistência real (sem EF Core ou banco de dados).

🧱 Estrutura do Projeto
Gamification.sln
│
├─ src/
│  └─ Gamification.Domain/
│     ├─ Awards/
│     │  └─ AwardBadgeService.cs
│     ├─ Policies/
│     │  └─ BonusPolicy.cs
│     ├─ Ports/
│     │  ├─ IAwardsReadStore.cs
│     │  └─ IAwardsWriteStore.cs
│     ├─ Model/
│     │  ├─ Badge.cs
│     │  ├─ RewardLog.cs
│     │  └─ XpAmount.cs
│     └─ Exceptions/
│        └─ DomainExceptions.cs
│
└─ tests/
   └─ Gamification.Domain.Tests/
      ├─ Fakes/
      │  └─ FakeStores.cs
      ├─ AwardBadgeServiceTests.cs
      └─ BonusPolicyTests.cs

🚀 Como Executar

Pré-requisitos
Ter o .NET SDK 9.0 instalado.
(Pode ser verificado com dotnet --version).

Restaurar dependências

dotnet restore


Executar os testes

dotnet test

🧩 Funcionalidades Implementadas
Regra	Descrição	Teste correspondente
Unicidade	Uma badge não pode ser concedida mais de uma vez ao mesmo estudante na mesma missão.	RepetirConcessao_mesma_badge_para_mesmo_estudante_deve_ser_idempotente_por_chave_natural
Elegibilidade	Apenas estudantes que concluíram a missão podem receber a badge.	ConcederBadge_sem_concluir_missao_deve_falhar
Janelas de Bônus	Determina o XP concedido de acordo com as datas do tema.	ConcederBadge_ate_bonusFullWeightEndDate_concede_bonus_integral, ConcederBadge_entre_fullWeight_e_finalDate_concede_bonus_reduzido, ConcederBadge_apos_bonusFinalDate_nao_concede_bonus_mas_concede_badge
Idempotência	Requisições repetidas não duplicam concessões.	Simulada nos testes por chave natural.
Atomicidade	Badge e XP são gravados juntos (simulado via FakeAwardsWriteStore).	Todos os testes de sucesso.
Auditoria	Cada concessão gera um RewardLog com motivo e horário.	Validado pelos asserts sobre write.LastLog.
🧠 Decisões de Design

Arquitetura Hexagonal (Ports and Adapters)

IAwardsReadStore e IAwardsWriteStore representam interfaces de leitura e gravação.

São substituídas por fakes nos testes para manter o domínio puro e testável.

TDD (Red → Green → Refactor)
Cada regra foi escrita como teste primeiro (fase “Red”), seguida pela implementação mínima para passar (fase “Green”) e por refatoração para clareza e coesão (fase “Refactor”).

Design por Intenção

O domínio expõe apenas AwardBadgeService.AwardBadge(...).

Nenhuma propriedade pública perigosa ou setters expostos.

Imutabilidade

Badge, XpAmount e RewardLog são records imutáveis.

Garante consistência e segurança em memória.

Mensagens de Erro Claras
Exceções de domínio (NotEligibleException, AlreadyAwardedException, etc.) possuem mensagens diretas, úteis para debugging e logs.

🧾 Casos de Bônus (BonusPolicy)

A política de bônus é independente e puramente funcional.

Situação	Resultado	Motivo
now <= bonusFullWeightEndDate	XP integral	"janela integral"
bonusFullWeightEndDate < now <= bonusFinalDate	XP reduzido	"janela reduzida"
now > bonusFinalDate	0 XP	"fora da janela de bônus"
Datas inválidas	ConfigurationException	"Datas de bônus inconsistentes."
📊 Cobertura de Testes

O conjunto cobre todos os cenários obrigatórios do enunciado:

 Concessão básica

 Idempotência

 Elegibilidade

 Janelas de bônus (integral / reduzido / fora)

 Exceções e mensagens de domínio

 Política pura de bônus

🧩 Execução Esperada

Ao rodar:

dotnet test


Saída esperada:

Passed!  Total tests: 7. Passed: 7. Failed: 0. Skipped: 0.

👨‍💻 Autor

Nome: Hilário Canci Neto
Disciplina: Programação Orientada a Objetos 4 — 2025/2
Professor: Everton Coimbra
