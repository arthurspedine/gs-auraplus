using AuraPlus.Web.Models;
using AuraPlus.Web.Models.DTOs.Equipe;
using AuraPlus.Web.Repositories;

namespace AuraPlus.Web.Services;

/// <summary>
/// Serviço de gerenciamento de equipes
/// </summary>
public class EquipeService : IEquipeService
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EquipeService> _logger;

    public EquipeService(
        IEquipeRepository equipeRepository,
        IUserRepository userRepository,
        ILogger<EquipeService> logger)
    {
        _equipeRepository = equipeRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova equipe e torna o usuário GESTOR
    /// </summary>
    public async Task<EquipeDTO> CreateEquipeAsync(int userId, CreateEquipeDTO createDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (user.Ativo == '0')
            throw new InvalidOperationException("Usuário está inativo.");

        // Verifica se usuário já está em uma equipe
        if (user.IdEquipe.HasValue)
            throw new InvalidOperationException("Usuário já está vinculado a uma equipe.");

        // Cria a nova equipe
        var equipe = new Equipe
        {
            NmTime = createDto.NmTime,
            Descricao = createDto.Descricao
        };

        equipe = await _equipeRepository.AddAsync(equipe);

        // Atualiza o usuário para GESTOR e vincula à equipe
        user.Role = "GESTOR";
        user.IdEquipe = equipe.Id;
        user.Cargo = createDto.Cargo;
        user.DataAdmissao = createDto.DataAdmissao ?? DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Equipe criada: {NomeEquipe} (ID: {EquipeId}) por {Usuario} (ID: {UserId})", 
            equipe.NmTime, equipe.Id, user.Nome, user.Id);

        // Recarrega equipe com usuários
        equipe = await _equipeRepository.GetByIdAsync(equipe.Id) ?? equipe;
        
        return EquipeDTO.FromEquipe(equipe);
    }

    /// <summary>
    /// Usuário entra em uma equipe existente e vira EMPREGADO
    /// </summary>
    public async Task<EquipeDTO> EntrarEquipeAsync(int userId, int equipeId, EntrarEquipeDTO entrarDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (user.Ativo == '0')
            throw new InvalidOperationException("Usuário está inativo.");

        // Verifica se usuário já está em uma equipe
        if (user.IdEquipe.HasValue)
            throw new InvalidOperationException("Usuário já está vinculado a uma equipe.");

        var equipe = await _equipeRepository.GetByIdAsync(equipeId);
        
        if (equipe == null)
            throw new KeyNotFoundException("Equipe não encontrada.");

        // Atualiza o usuário para EMPREGADO e vincula à equipe
        user.Role = "EMPREGADO";
        user.IdEquipe = equipe.Id;
        user.Cargo = entrarDto.Cargo;
        user.DataAdmissao = entrarDto.DataAdmissao ?? DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Usuário {Usuario} (ID: {UserId}) entrou na equipe {Equipe} (ID: {EquipeId})", 
            user.Nome, user.Id, equipe.NmTime, equipe.Id);

        // Recarrega equipe com usuários atualizados
        equipe = await _equipeRepository.GetByIdAsync(equipe.Id) ?? equipe;
        
        return EquipeDTO.FromEquipe(equipe);
    }

    /// <summary>
    /// Usuário sai da equipe e volta a ser NOVO_USUARIO
    /// </summary>
    public async Task<bool> SairEquipeAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (!user.IdEquipe.HasValue)
            throw new InvalidOperationException("Usuário não está vinculado a nenhuma equipe.");
  
        var equipeId = user.IdEquipe.Value;

        // Verifica se a equipe ficou sem membros e deleta
        var hasMembros = await _equipeRepository.HasMembrosAsync(equipeId);

        if (user.Cargo == "GESTOR") 
          if (hasMembros) 
            throw new InvalidOperationException("Gestor não pode sair da equipe enquanto houver membros. Remova todos os membros primeiro.");
        

        // Remove vinculação da equipe
        user.IdEquipe = null;
        user.Role = "NOVO_USUARIO";
        user.Cargo = null;
        user.DataAdmissao = null;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Usuário {Usuario} (ID: {UserId}) saiu da equipe (ID: {EquipeId})", 
            user.Nome, user.Id, equipeId);

        if (!hasMembros)
        {
            await _equipeRepository.DeleteAsync(equipeId);
            _logger.LogInformation("Equipe (ID: {EquipeId}) deletada por não ter mais membros", equipeId);
        }

        return true;
    }

    /// <summary>
    /// Busca equipe por ID
    /// </summary>
    public async Task<EquipeDTO?> GetEquipeByIdAsync(int equipeId)
    {
        var equipe = await _equipeRepository.GetByIdAsync(equipeId);
        return equipe != null ? EquipeDTO.FromEquipe(equipe) : null;
    }

    /// <summary>
    /// Lista todas as equipes
    /// </summary>
    public async Task<IEnumerable<EquipeDTO>> GetAllEquipesAsync()
    {
        var equipes = await _equipeRepository.GetAllAsync();
        return equipes.Select(e => EquipeDTO.FromEquipe(e));
    }

    /// <summary>
    /// Atualiza informações da equipe (apenas GESTOR pode)
    /// </summary>
    public async Task<EquipeDTO> UpdateEquipeAsync(int equipeId, int userId, UpdateEquipeDTO updateDto)
    {
        var equipe = await _equipeRepository.GetByIdAsync(equipeId);
        
        if (equipe == null)
            throw new KeyNotFoundException("Equipe não encontrada.");

        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        // Verifica se o usuário é GESTOR da equipe
        if (user.Role != "GESTOR" || user.IdEquipe != equipeId)
            throw new UnauthorizedAccessException("Apenas o gestor da equipe pode atualizá-la.");

        // Atualiza campos se fornecidos
        if (!string.IsNullOrWhiteSpace(updateDto.NmTime))
            equipe.NmTime = updateDto.NmTime;

        if (updateDto.Descricao != null)
            equipe.Descricao = updateDto.Descricao;

        await _equipeRepository.UpdateAsync(equipe);

        _logger.LogInformation("Equipe {Equipe} (ID: {EquipeId}) atualizada por {Usuario} (ID: {UserId})", 
            equipe.NmTime, equipe.Id, user.Nome, user.Id);

        // Recarrega equipe com usuários
        equipe = await _equipeRepository.GetByIdAsync(equipe.Id) ?? equipe;
        
        return EquipeDTO.FromEquipe(equipe);
    }

    /// <summary>
    /// Deleta equipe (apenas GESTOR pode e se não houver membros)
    /// </summary>
    public async Task<bool> DeleteEquipeAsync(int equipeId, int userId)
    {
        var equipe = await _equipeRepository.GetByIdAsync(equipeId);
        
        if (equipe == null)
            throw new KeyNotFoundException("Equipe não encontrada.");

        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        // Verifica se o usuário é GESTOR da equipe
        if (user.Role != "GESTOR" || user.IdEquipe != equipeId)
            throw new UnauthorizedAccessException("Apenas o gestor da equipe pode deletá-la.");

        // Verifica se há outros membros além do gestor
        var totalMembros = await _equipeRepository.GetTotalMembrosAsync(equipeId);
        if (totalMembros > 1)
            throw new InvalidOperationException("Não é possível deletar equipe com outros membros. Remova todos os membros primeiro.");

        // Remove vinculação do gestor
        user.IdEquipe = null;
        user.Role = "NOVO_USUARIO";
        user.Cargo = null;
        user.DataAdmissao = null;
        await _userRepository.UpdateAsync(user);

        // Deleta a equipe
        await _equipeRepository.DeleteAsync(equipeId);

        _logger.LogInformation("Equipe {Equipe} (ID: {EquipeId}) deletada por {Usuario} (ID: {UserId})", 
            equipe.NmTime, equipe.Id, user.Nome, user.Id);

        return true;
    }

    /// <summary>
    /// Gestor adiciona um membro à sua equipe
    /// </summary>
    public async Task<EquipeDTO> AdicionarMembroAsync(int gestorId, int membroId, string cargo, DateTime? dataAdmissao = null)
    {
        var gestor = await _userRepository.GetByIdAsync(gestorId);
        
        if (gestor == null)
            throw new KeyNotFoundException("Gestor não encontrado.");

        // Verifica se o usuário é GESTOR e está vinculado a uma equipe
        if (gestor.Role != "GESTOR")
            throw new UnauthorizedAccessException("Apenas gestores podem adicionar membros.");

        if (!gestor.IdEquipe.HasValue)
            throw new InvalidOperationException("Gestor não está vinculado a nenhuma equipe.");

        var equipeId = gestor.IdEquipe.Value;
        var equipe = await _equipeRepository.GetByIdAsync(equipeId);
        
        if (equipe == null)
            throw new KeyNotFoundException("Equipe não encontrada.");

        var membro = await _userRepository.GetByIdAsync(membroId);
        
        if (membro == null)
            throw new KeyNotFoundException("Membro não encontrado.");

        if (membro.Ativo == '0')
            throw new InvalidOperationException("Membro está inativo.");

        // Verifica se membro já está em uma equipe
        if (membro.IdEquipe.HasValue)
            throw new InvalidOperationException("Membro já está vinculado a uma equipe.");

        // Adiciona membro à equipe
        membro.IdEquipe = equipeId;
        membro.Role = "EMPREGADO";
        membro.Cargo = cargo;
        membro.DataAdmissao = dataAdmissao ?? DateTime.UtcNow;

        await _userRepository.UpdateAsync(membro);

        _logger.LogInformation("Membro {Membro} (ID: {MembroId}) adicionado à equipe {Equipe} (ID: {EquipeId}) por {Gestor} (ID: {GestorId})", 
            membro.Nome, membro.Id, equipe.NmTime, equipe.Id, gestor.Nome, gestor.Id);

        // Recarrega equipe com usuários atualizados
        equipe = await _equipeRepository.GetByIdAsync(equipe.Id) ?? equipe;
        
        return EquipeDTO.FromEquipe(equipe);
    }

    /// <summary>
    /// Gestor remove um membro da sua equipe
    /// </summary>
    public async Task<bool> RemoverMembroAsync(int gestorId, int membroId)
    {
        var gestor = await _userRepository.GetByIdAsync(gestorId);
        
        if (gestor == null)
            throw new KeyNotFoundException("Gestor não encontrado.");

        // Verifica se o usuário é GESTOR e está vinculado a uma equipe
        if (gestor.Role != "GESTOR")
            throw new UnauthorizedAccessException("Apenas gestores podem remover membros.");

        if (!gestor.IdEquipe.HasValue)
            throw new InvalidOperationException("Gestor não está vinculado a nenhuma equipe.");

        var equipeId = gestor.IdEquipe.Value;
        var equipe = await _equipeRepository.GetByIdAsync(equipeId);
        
        if (equipe == null)
            throw new KeyNotFoundException("Equipe não encontrada.");

        var membro = await _userRepository.GetByIdAsync(membroId);
        
        if (membro == null)
            throw new KeyNotFoundException("Membro não encontrado.");

        // Não pode remover o próprio gestor
        if (membro.Id == gestorId)
            throw new InvalidOperationException("Gestor não pode remover a si mesmo. Use o endpoint de sair da equipe.");

        // Verifica se membro está na equipe
        if (membro.IdEquipe != equipeId)
            throw new InvalidOperationException("Membro não pertence a esta equipe.");

        // Remove membro da equipe
        membro.IdEquipe = null;
        membro.Role = "NOVO_USUARIO";
        membro.Cargo = null;
        membro.DataAdmissao = null;

        await _userRepository.UpdateAsync(membro);

        _logger.LogInformation("Membro {Membro} (ID: {MembroId}) removido da equipe {Equipe} (ID: {EquipeId}) por {Gestor} (ID: {GestorId})", 
            membro.Nome, membro.Id, equipe.NmTime, equipe.Id, gestor.Nome, gestor.Id);

        return true;
    }
}
