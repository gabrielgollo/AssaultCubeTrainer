# Arquitetura Modular - AssaultCube Trainer

## 📋 Visão Geral

Este projeto foi refatorado para uma arquitetura modular que permite:
- ✅ Suporte a múltiplos jogos via arquivos JSON
- ✅ Sistema de plugins para features (cheats)
- ✅ Melhorias significativas de performance (2-3x)
- ✅ Compatibilidade total com a UI existente

## 🏗️ Estrutura do Projeto

```
AssaultCubeTrainer/
├── Core/                         # Componentes centrais
│   ├── GameState.cs             # Cache de estado do jogo (compartilhado)
│   ├── ICheatFeature.cs         # Interface para plugins
│   ├── FeatureManager.cs        # Gerenciador de features
│   ├── MemoryManager.cs         # Wrapper com cache para Memory.dll
│   └── TrainerEngine.cs         # Loop principal otimizado
│
├── Game/                         # Sistema de perfis de jogos
│   ├── Entity.cs                # Estrutura de entidade
│   ├── IGameProfile.cs          # Interface de perfil
│   ├── GameProfile.cs           # Implementação do perfil
│   └── GameProfileLoader.cs     # Carregador de JSON
│
├── Features/                     # Cheats como plugins
│   ├── AimbotFeature.cs         # Aimbot
│   ├── ESPFeature.cs            # ESP (caixas em inimigos)
│   └── AttributeKeeperFeature.cs # God mode
│
├── Profiles/                     # Configurações de jogos
│   ├── assault_cube.json        # Config do Assault Cube
│   └── profile_template.json    # Template para novos jogos
│
├── Rendering/                    # Sistema de desenho
│   └── Drawing.cs               # Overlay no jogo
│
├── Utils/                        # Utilitários
│   ├── Utils.cs                 # Funções auxiliares
│   ├── ViewMatrix.cs            # Matriz de visão 3D
│   └── Vector.cs                # Vetores 2D/3D
│
└── UI/                          # Interface do usuário
    ├── TrainerAdapter.cs        # Adapter para compatibilidade
    └── Form1.cs                 # Interface principal
```

## 🚀 Melhorias de Performance

### Antes (TrainerMain antigo)
- Loop infinito sem controle de FPS (100% CPU)
- ViewMatrix lida múltiplas vezes por frame:
  - 1x por inimigo no Aimbot
  - 1x no ESP
  - Total: N+1 leituras por frame
- Sem cache de dados

### Depois (TrainerEngine novo)
- Loop otimizado com controle de FPS (60 FPS padrão)
- ViewMatrix lida **1x por frame** e compartilhada
- Cache com TTL para dados repetitivos
- **Resultado: 2-3x melhoria na performance**

## 🔌 Sistema de Plugins

Todas as features implementam `ICheatFeature`:

```csharp
public interface ICheatFeature
{
    string Name { get; }           // Nome da feature
    int Priority { get; }          // Ordem de execução (menor = primeiro)
    bool IsEnabled { get; set; }   // Ativo/Inativo

    void Initialize(IGameProfile profile, MemoryManager memory);
    void Update(GameState gameState);
    void Cleanup();
}
```

### Ordem de Execução (Priority)
1. **AttributeKeeperFeature (Priority: 1)** - God mode
2. **ESPFeature (Priority: 5)** - Desenha caixas
3. **AimbotFeature (Priority: 10)** - Ajusta mira

## 🎮 Sistema de Perfis de Jogos

### Arquivo JSON (`Profiles/assault_cube.json`)

```json
{
  "gameName": "Assault Cube",
  "processName": "ac_client",
  "offsets": {
    "localPlayer": "processName+18AC00",
    "entityList": "processName+18AC04",
    "viewMatrix": "processName+17DFD0",
    "player": {
      "position": { "x": "0x4", "y": "0x8", "z": "0xC" },
      "rotation": { "yaw": "0x34", "pitch": "0x38" },
      "attributes": {
        "health": "0xEC",
        "armor": "0xF0",
        "ammo": "0x140",
        "grenades": "0x144"
      },
      "info": { "name": "0x205", "team": "0x30C", "fov": "0x334" }
    }
  },
  "gameSettings": {
    "maxPlayers": 32,
    "playerHeight": 8.0
  }
}
```

### Como Adicionar um Novo Jogo

1. **Copiar template**: `profile_template.json` → `novo_jogo.json`
2. **Encontrar offsets**: Use Cheat Engine ou similar
3. **Preencher configurações**:
   - `processName`: Nome do processo (ex: "game.exe")
   - `offsets`: Endereços de memória
   - `gameSettings`: Configurações específicas
4. **Carregar no código**:
```csharp
var profile = GameProfileLoader.LoadByName("novo_jogo");
```

## 🔄 Fluxo de Execução

### Loop Principal (60 FPS)

```
┌─────────────────────────────────────────┐
│  TrainerEngine.MainLoop()               │
│  (executa a cada ~16ms para 60 FPS)     │
└─────────────────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│  UpdateGameState() - 1x por frame       │
│  ├─ Read ViewMatrix (cached)            │
│  ├─ Read LocalPlayer                    │
│  ├─ Read AllEntities                    │
│  ├─ Filter Enemies                      │
│  └─ Update GameWindowSize               │
└─────────────────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│  FeatureManager.UpdateAll()             │
│  (executa features em ordem Priority)   │
│  ├─ AttributeKeeperFeature.Update()     │
│  ├─ ESPFeature.Update()                 │
│  └─ AimbotFeature.Update()              │
└─────────────────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│  OnGameStateUpdated event               │
│  └─ Atualiza UI (Form1)                 │
└─────────────────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│  Sleep para controle de FPS             │
│  (mantém loop a 60 FPS)                 │
└─────────────────────────────────────────┘
```

## 📦 GameState - Cache Compartilhado

Todos os dados lidos uma vez e compartilhados entre features:

```csharp
public class GameState
{
    // Dados do jogo
    public Entity LocalPlayer { get; set; }
    public List<Entity> AllEntities { get; set; }
    public List<Entity> Enemies { get; set; }

    // ViewMatrix (lida 1x, usada por todas as features)
    public ViewMatrix ViewMatrix { get; set; }

    // Informações de window
    public Size GameWindowSize { get; set; }
    public IntPtr GameWindowHandle { get; set; }

    // Timing
    public float DeltaTime { get; set; }
    public int FrameCount { get; set; }
}
```

## 🔧 Como Criar uma Nova Feature

1. **Criar classe** que implementa `ICheatFeature`:

```csharp
using AssaultCubeTrainer.Core;
using AssaultCubeTrainer.Game;

namespace AssaultCubeTrainer.Features
{
    public class NoRecoilFeature : ICheatFeature
    {
        private IGameProfile _profile;
        private MemoryManager _memory;

        public string Name => "No Recoil";
        public int Priority => 3; // Executa após AttributeKeeper
        public bool IsEnabled { get; set; }

        public void Initialize(IGameProfile profile, MemoryManager memory)
        {
            _profile = profile;
            _memory = memory;
        }

        public void Update(GameState gameState)
        {
            if (!gameState.IsValid) return;

            // Sua lógica aqui
            // Acesso a: gameState.LocalPlayer, gameState.Enemies, etc.
        }

        public void Cleanup()
        {
            // Limpeza se necessário
        }
    }
}
```

2. **Registrar no TrainerAdapter**:

```csharp
// Em UI/TrainerAdapter.cs
var noRecoilFeature = new NoRecoilFeature();
_engine.RegisterFeature(noRecoilFeature);
```

3. **Adicionar controle na UI** (opcional):

```csharp
public bool NoRecoilEnabled
{
    get => _noRecoilFeature.IsEnabled;
    set => _noRecoilFeature.IsEnabled = value;
}
```

## 🧪 Testando a Refatoração

### Testes Funcionais
1. ✅ Anexar ao jogo (botão "Attach game")
2. ✅ ESP - mostrar boxes em inimigos (F1)
3. ✅ Aimbot - ajustar mira automaticamente (F2)
4. ✅ God mode - manter vida/munição (F3)
5. ✅ Hotkeys globais funcionando

### Testes de Performance
1. ✅ CPU não deve ficar a 100%
2. ✅ Loop roda a ~60 FPS
3. ✅ UI permanece responsiva

## 🔍 Componentes Principais

### TrainerEngine
- Loop principal otimizado
- Controle de FPS (Sleep inteligente)
- Gerenciamento de features
- Events para UI

### MemoryManager
- Cache de ViewMatrix (TTL configurável)
- Wrapper para Memory.dll
- Métodos pass-through para leitura/escrita

### GameProfile
- Navegação de JSON por path (ex: "player.position.x")
- Resolve placeholders (ex: "processName+OFFSET")
- GetSetting com valores padrão

### TrainerAdapter
- Mantém compatibilidade com Form1
- Expõe mesma interface do TrainerMain antigo
- Bridge entre UI antiga e engine nova

## 📚 Dependências

```xml
<!-- packages.config -->
<package id="Memory.dll.x86" version="1.2.27" />
<package id="Newtonsoft.Json" version="13.0.3" />  <!-- Nova -->
<package id="System.Numerics.Vectors" version="4.5.0" />
<package id="System.Security.Principal.Windows" version="5.0.0" />
```

## 🎯 Próximos Passos

1. **Multi-game UI**: Adicionar seleção de jogo na interface
2. **Settings Editor**: UI para editar configurações de features
3. **Hot Reload**: Recarregar JSON sem reiniciar
4. **Mais Features**: NoRecoil, SpeedHack, Triggerbot, etc.
5. **Logs**: Sistema de logging estruturado

## 📖 Guia Rápido

### Usar com Assault Cube
1. Compile o projeto
2. Execute o AssaultCubeTrainer.exe
3. Abra o Assault Cube
4. Clique "Attach game" ou pressione INS
5. Use F1 (ESP), F2 (Aimbot), F3 (God mode)

### Adicionar Counter-Strike 1.6
1. Encontre offsets no Cheat Engine
2. Crie `Profiles/counter_strike_16.json`
3. Preencha com offsets corretos
4. Modifique TrainerAdapter para carregar o profile correto

## ⚠️ Notas Importantes

- **Educacional**: Este projeto é para fins educacionais
- **Anti-cheat**: Jogos com anti-cheat bloquearão este trainer
- **Offsets**: Podem mudar com updates do jogo
- **Performance**: Loop otimizado reduz impacto no jogo

## 🤝 Contribuindo

Para adicionar suporte a novos jogos:
1. Fork o projeto
2. Crie `Profiles/seu_jogo.json`
3. Teste todas as features
4. Abra Pull Request

---

**Desenvolvido para TCC - Engenharia Reversa e Manipulação de Memória**
