FROM chainguard/dotnet-sdk:latest AS build
WORKDIR /app
COPY --chown=nonroot:nonroot . .
RUN dotnet --info
RUN cd src/Web && dotnet publish -c Release -p:PublishSingleFile=true -p:SelfContained=true -p:PublishReadyToRun=true -p:InvariantGlobalization=true --use-current-runtime --output ../../artifacts

FROM chainguard/wolfi-base:latest as runner
WORKDIR /app
RUN apk add --no-cache tini libstdc++ libgcc openssl \
    && rm -rf /var/cache/apk/* \
    && rm -f /sbin/apk /bin/sh /bin/busybox   # Closing the door behind us
USER nonroot
COPY --from=build /app/artifacts/ .
ENTRYPOINT ["tini", "--", "./OmniRepo.Web"]