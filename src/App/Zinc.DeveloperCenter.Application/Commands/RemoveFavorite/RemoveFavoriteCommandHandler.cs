using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zinc.DeveloperCenter.Domain.Services.Favorites;

namespace Zinc.DeveloperCenter.Application.Commands.RemoveFavorite
{
    internal class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
    {
        private readonly IFavoritesService favorites;

        public RemoveFavoriteCommandHandler(IFavoritesService favorites)
        {
            this.favorites = favorites;
        }

        public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            await favorites.RemoveFavorite(request.TenantId, request.ApplicationName, request.FilePath).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
