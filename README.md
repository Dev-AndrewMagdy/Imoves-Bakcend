# Movies API

ASP.NET Core backend that proxies TMDB movie data. The Android/client app calls this API;
it never calls TMDB directly.

## Endpoints

- `GET /api/movies?page=1&language=en-US`
- `GET /api/movies/{movieId}?language=en-US` (for example, `GET /api/movies/969681`)

Successful TMDB payloads are mapped to typed models (`MovieList` for the listing and
`MovieDetails` for a single movie) before being returned. Upstream error status codes,
error bodies, and relevant headers are preserved.

## Configuration

Non-secret TMDB settings live in `Movies.Api/appsettings.json`:

```json
"Tmdb": {
  "BaseUrl": "https://api.themoviedb.org/3/",
  "ImageBaseUrl": "https://image.tmdb.org/t/p/",
  "TimeoutSeconds": 15,
  "ReadAccessToken": ""
}
```

The TMDB API Read Access Token is never stored in the repository. Leave
`ReadAccessToken` empty in `appsettings.json` and provide it through user secrets
(Development) or an environment variable:

```powershell
dotnet user-secrets set "Tmdb:ReadAccessToken" "<your-read-access-token>" --project Movies.Api
```

or:

```powershell
$env:Tmdb__ReadAccessToken = "<your-read-access-token>"
```

`Tmdb:BaseUrl` defaults to `https://api.themoviedb.org/3/` and `Tmdb:TimeoutSeconds`
defaults to `15`. The application fails at startup if the token is missing or the base URL
is not HTTPS.

## Run

```powershell
dotnet run --project Movies.Api
```

Swagger UI is available in Development at `https://localhost:7110/swagger` (or the HTTP URL
shown by the launch profile). The generated document is at
`/swagger/v1/swagger.json`.

## Tests

```powershell
dotnet test
```
