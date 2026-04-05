using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MCT.Models;

public partial class MctContext : DbContext
{
    public MctContext() { }
    public MctContext(DbContextOptions<MctContext> options) : base(options) { }

    public virtual DbSet<Match> Matches { get; set; }
    public virtual DbSet<MatchType> MatchTypes { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }
    public virtual DbSet<Player> Players { get; set; }
    public virtual DbSet<Stat> Stats { get; set; }
    public virtual DbSet<Team> Teams { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }
    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
    public virtual DbSet<TournamentStatus> TournamentStatuses { get; set; }
    public virtual DbSet<TournamentTeam> TournamentTeams { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=MCT;Username=postgres;Password=Alex1111");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("matches_pkey");
            entity.ToTable("matches");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.MatchType).HasMaxLength(10).HasColumnName("match_type");
            entity.Property(e => e.ScheduledAt).HasColumnName("scheduled_at");
            entity.Property(e => e.ScoreA).HasColumnName("score_a");
            entity.Property(e => e.ScoreB).HasColumnName("score_b");
            entity.Property(e => e.TeamAId).HasColumnName("team_a_id");
            entity.Property(e => e.TeamBId).HasColumnName("team_b_id");
            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.WinnerId).HasColumnName("winner_id");

            entity.HasOne(d => d.MatchTypeNavigation).WithMany(p => p.Matches).HasForeignKey(d => d.MatchType).HasConstraintName("matches_match_type_fkey");
            entity.HasOne(d => d.TeamA).WithMany(p => p.MatchTeamAs).HasForeignKey(d => d.TeamAId).HasConstraintName("matches_team_a_id_fkey");
            entity.HasOne(d => d.TeamB).WithMany(p => p.MatchTeamBs).HasForeignKey(d => d.TeamBId).HasConstraintName("matches_team_b_id_fkey");
            entity.HasOne(d => d.Tournament).WithMany(p => p.Matches).HasForeignKey(d => d.TournamentId).HasConstraintName("matches_tournament_id_fkey");
            entity.HasOne(d => d.Winner).WithMany(p => p.MatchWinners).HasForeignKey(d => d.WinnerId).HasConstraintName("matches_winner_id_fkey");
        });

        modelBuilder.Entity<MatchType>(entity => { entity.HasKey(e => e.TypeName).HasName("match_types_pkey"); entity.ToTable("match_types"); entity.Property(e => e.TypeName).HasMaxLength(10).HasColumnName("type_name"); });
        modelBuilder.Entity<Payment>(entity => { entity.HasKey(e => e.PaymentId).HasName("payments_pkey"); entity.ToTable("payments"); entity.Property(e => e.PaymentId).HasColumnName("payment_id"); entity.Property(e => e.Amount).HasPrecision(10, 2).HasColumnName("amount"); entity.Property(e => e.PaymentDate).HasColumnName("payment_date"); entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status"); entity.Property(e => e.TicketId).HasColumnName("ticket_id"); entity.Property(e => e.TransactionId).HasMaxLength(100).HasColumnName("transaction_id"); entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Payments).HasForeignKey(d => d.Status).HasConstraintName("payments_status_fkey"); entity.HasOne(d => d.Ticket).WithMany(p => p.Payments).HasForeignKey(d => d.TicketId).HasConstraintName("payments_ticket_id_fkey"); });
        modelBuilder.Entity<PaymentStatus>(entity => { entity.HasKey(e => e.StatusName).HasName("payment_statuses_pkey"); entity.ToTable("payment_statuses"); entity.Property(e => e.StatusName).HasMaxLength(20).HasColumnName("status_name"); });
        modelBuilder.Entity<Player>(entity => { entity.HasKey(e => e.PlayerId).HasName("players_pkey"); entity.ToTable("players"); entity.Property(e => e.PlayerId).HasColumnName("player_id"); entity.Property(e => e.TeamId).HasColumnName("team_id"); entity.Property(e => e.UserId).HasColumnName("user_id"); entity.HasOne(d => d.Team).WithMany(p => p.Players).HasForeignKey(d => d.TeamId).HasConstraintName("players_team_id_fkey"); entity.HasOne(d => d.User).WithMany(p => p.Players).HasForeignKey(d => d.UserId).HasConstraintName("players_user_id_fkey"); });
        modelBuilder.Entity<Stat>(entity => { entity.HasKey(e => e.StatId).HasName("stats_pkey"); entity.ToTable("stats"); entity.Property(e => e.StatId).HasColumnName("stat_id"); entity.Property(e => e.Assists).HasColumnName("assists"); entity.Property(e => e.Deaths).HasColumnName("deaths"); entity.Property(e => e.HsPercentage).HasPrecision(5, 2).HasColumnName("hs_percentage"); entity.Property(e => e.Kills).HasColumnName("kills"); entity.Property(e => e.MatchId).HasColumnName("match_id"); entity.Property(e => e.PlayerId).HasColumnName("player_id"); entity.HasOne(d => d.Match).WithMany(p => p.Stats).HasForeignKey(d => d.MatchId).HasConstraintName("stats_match_id_fkey"); entity.HasOne(d => d.Player).WithMany(p => p.Stats).HasForeignKey(d => d.PlayerId).HasConstraintName("stats_player_id_fkey"); });
        modelBuilder.Entity<Team>(entity => { entity.HasKey(e => e.TeamId).HasName("teams_pkey"); entity.ToTable("teams"); entity.Property(e => e.TeamId).HasColumnName("team_id"); entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name"); entity.Property(e => e.Region).HasMaxLength(50).HasColumnName("region"); entity.Property(e => e.ShortCode).HasMaxLength(3).IsFixedLength().HasColumnName("short_code"); });
        modelBuilder.Entity<Ticket>(entity => { entity.HasKey(e => e.TicketId).HasName("tickets_pkey"); entity.ToTable("tickets"); entity.Property(e => e.TicketId).HasColumnName("ticket_id"); entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date"); entity.Property(e => e.QrCode).HasMaxLength(255).HasColumnName("qr_code"); entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status"); entity.Property(e => e.TournamentId).HasColumnName("tournament_id"); entity.Property(e => e.UserId).HasColumnName("user_id"); entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Tickets).HasForeignKey(d => d.Status).HasConstraintName("tickets_status_fkey"); entity.HasOne(d => d.Tournament).WithMany(p => p.Tickets).HasForeignKey(d => d.TournamentId).HasConstraintName("tickets_tournament_id_fkey"); entity.HasOne(d => d.User).WithMany(p => p.Tickets).HasForeignKey(d => d.UserId).HasConstraintName("tickets_user_id_fkey"); });
        modelBuilder.Entity<TicketStatus>(entity => { entity.HasKey(e => e.StatusName).HasName("ticket_statuses_pkey"); entity.ToTable("ticket_statuses"); entity.Property(e => e.StatusName).HasMaxLength(20).HasColumnName("status_name"); });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.TournamentId).HasName("tournaments_pkey");
            entity.ToTable("tournaments");
            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Location).HasMaxLength(100).HasColumnName("location");
            entity.Property(e => e.Price).HasPrecision(10, 2).HasColumnName("price");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
            entity.Property(e => e.Places).HasColumnName("places");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Tournaments).HasForeignKey(d => d.Status).HasConstraintName("tournaments_status_fkey");
        });

        modelBuilder.Entity<TournamentStatus>(entity => { entity.HasKey(e => e.StatusName).HasName("tournament_statuses_pkey"); entity.ToTable("tournament_statuses"); entity.Property(e => e.StatusName).HasMaxLength(20).HasColumnName("status_name"); });
        modelBuilder.Entity<TournamentTeam>(entity => { entity.HasKey(e => e.Id).HasName("tournament_teams_pkey"); entity.ToTable("tournament_teams"); entity.Property(e => e.Id).HasColumnName("id"); entity.Property(e => e.Placement).HasColumnName("placement"); entity.Property(e => e.TeamId).HasColumnName("team_id"); entity.Property(e => e.TournamentId).HasColumnName("tournament_id"); entity.HasOne(d => d.Team).WithMany(p => p.TournamentTeams).HasForeignKey(d => d.TeamId).HasConstraintName("tournament_teams_team_id_fkey"); entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentTeams).HasForeignKey(d => d.TournamentId).HasConstraintName("tournament_teams_tournament_id_fkey"); });
        modelBuilder.Entity<User>(entity => { entity.HasKey(e => e.UserId).HasName("users_pkey"); entity.ToTable("users"); entity.Property(e => e.UserId).HasColumnName("user_id"); entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email"); entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash"); entity.Property(e => e.Role).HasMaxLength(20).HasColumnName("role"); entity.Property(e => e.Username).HasMaxLength(255).HasColumnName("username"); entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users).HasForeignKey(d => d.Role).HasConstraintName("users_role_fkey"); });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleName).HasName("user_roles_pkey");
            entity.ToTable("user_roles");
            entity.Property(e => e.RoleName).HasMaxLength(20).HasColumnName("role_name");

            entity.HasData(
                new UserRole { RoleName = "Admin" },
                new UserRole { RoleName = "Player" },
                new UserRole { RoleName = "User" }
            );
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}