FROM chainguard/dotnet-sdk:latest AS build
WORKDIR /app
COPY --chown=nonroot:nonroot . .
RUN dotnet --info
# -p:PublishSingleFile=true
RUN dotnet publish -c Release -p:PublishAot=true -p:UseAppHost=true -p:InvariantGlobalization=true --use-current-runtime

FROM chainguard/wolfi-base:latest as runner
WORKDIR /app
RUN apk add --no-cache tini
USER nonroot
COPY --from=build /app/artifacts/ .
ENTRYPOINT ["tini", "--", "./OmniRepo.Web"]