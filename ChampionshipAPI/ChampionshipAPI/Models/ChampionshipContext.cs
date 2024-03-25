using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChampionshipAPI.Models;

public partial class ChampionshipContext : DbContext
{
    public ChampionshipContext()
    {
    }

    public ChampionshipContext(DbContextOptions<ChampionshipContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Championship> Championships { get; set; }

    public virtual DbSet<ChampionshipStatus> ChampionshipStatuses { get; set; }

    public virtual DbSet<ChampionshipType> ChampionshipTypes { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameStatus> GameStatuses { get; set; }

    public virtual DbSet<GameType> GameTypes { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamPlayer> TeamPlayers { get; set; }

    public virtual DbSet<TeamType> TeamTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=admin;Database=Championship");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Championship>(entity =>
        {
            entity.HasIndex(e => e.ChampionshipStatusId, "IX_Championships_ChampionshipStatusId");

            entity.HasIndex(e => e.ChampionshipTypeId, "IX_Championships_ChampionshipTypeId");

            entity.HasIndex(e => e.GameTypeId, "IX_Championships_GameTypeId");

            entity.HasIndex(e => e.WinnerId, "IX_Championships_WinnerId");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.ChampionshipStatus).WithMany(p => p.Championships).HasForeignKey(d => d.ChampionshipStatusId);

            entity.HasOne(d => d.ChampionshipType).WithMany(p => p.Championships).HasForeignKey(d => d.ChampionshipTypeId);

            entity.HasOne(d => d.GameType).WithMany(p => p.Championships).HasForeignKey(d => d.GameTypeId);

            entity.HasOne(d => d.Winner).WithMany(p => p.Championships).HasForeignKey(d => d.WinnerId);
        });

        modelBuilder.Entity<ChampionshipType>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasIndex(e => e.BlueTeamId, "IX_Games_BlueTeamID");

            entity.HasIndex(e => e.GameStatusId, "IX_Games_GameStatusID");

            entity.HasIndex(e => e.GameTypeId, "IX_Games_GameTypeID");

            entity.HasIndex(e => e.RedTeamId, "IX_Games_RedTeamID");

            entity.Property(e => e.BlueTeamId).HasColumnName("BlueTeamID");
            entity.Property(e => e.GameStatusId).HasColumnName("GameStatusID");
            entity.Property(e => e.GameTypeId).HasColumnName("GameTypeID");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.RedTeamId).HasColumnName("RedTeamID");

            entity.HasOne(d => d.BlueTeam).WithMany(p => p.GameBlueTeams).HasForeignKey(d => d.BlueTeamId);

            entity.HasOne(d => d.GameStatus).WithMany(p => p.Games).HasForeignKey(d => d.GameStatusId);

            entity.HasOne(d => d.GameType).WithMany(p => p.Games).HasForeignKey(d => d.GameTypeId);

            entity.HasOne(d => d.RedTeam).WithMany(p => p.GameRedTeams).HasForeignKey(d => d.RedTeamId);
        });

        modelBuilder.Entity<GameStatus>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<GameType>(entity =>
        {
            entity.HasIndex(e => e.TeamTypeId, "IX_GameTypes_TeamTypeId");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.TeamType).WithMany(p => p.GameTypes).HasForeignKey(d => d.TeamTypeId);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasIndex(e => e.TeamTypeId, "IX_Teams_TeamTypeId");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.TeamType).WithMany(p => p.Teams).HasForeignKey(d => d.TeamTypeId);
        });

        modelBuilder.Entity<TeamPlayer>(entity =>
        {
            entity.HasIndex(e => e.PlayerId, "IX_TeamPlayers_PlayerId");

            entity.HasIndex(e => e.TeamId, "IX_TeamPlayers_TeamId");

            //entity.HasOne(d => d.Player).WithMany(p => p.TeamPlayers).HasForeignKey(d => d.PlayerId);

            entity.HasOne(d => d.Team).WithMany(p => p.TeamPlayers).HasForeignKey(d => d.TeamId);
        });

        modelBuilder.Entity<TeamType>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
