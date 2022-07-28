using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zinc.DeveloperCenter.Domain.Services.Favorites;

namespace Zinc.DeveloperCenter.Application.Commands.AddFavorite
{
    internal class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Unit>
    {
        private readonly IFavoritesService favorites;

        public AddFavoriteCommandHandler(IFavoritesService favorites)
        {
            this.favorites = favorites;
        }

        public async Task<Unit> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            await favorites.AddFavorite(request.ApplicationName, request.FilePath).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
