FROM docker.io/chainguard/dotnet-sdk:latest AS build
WORKDIR /app
COPY --chown=nonroot:nonroot . .
RUN dotnet --info
RUN cd src/Web && dotnet publish -c Release -p:PublishSingleFile=true -p:SelfContained=true -p:PublishReadyToRun=true -p:InvariantGlobalization=true --use-current-runtime --output ../../artifacts

FROM docker.io/chainguard/wolfi-base:latest as runner
WORKDIR /app
RUN apk add --no-cache tini libstdc++ libgcc openssl \
    && rm -rf /var/cache/apk/* \
    && rm -f /sbin/apk /bin/sh /bin/busybox   # Closing the door behind us
USER nonroot
# The app should NEVER modify its own files. If it needs to write data, it should use a mounted volume.
COPY --from=build --chown=root:root /app/artifacts/ . 
ENTRYPOINT ["tini", "--", "./OmniRepo.Web"]