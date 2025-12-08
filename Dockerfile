FROM alpine:latest AS build
WORKDIR /app
COPY . .
RUN echo "http://dl-cdn.alpinelinux.org/alpine/edge/community" >> /etc/apk/repositories && \
apk add --no-cache dotnet10-sdk
RUN dotnet --info
RUN cd src/Web && dotnet publish -c Release -p:PublishSingleFile=true -p:SelfContained=true -p:PublishReadyToRun=true -p:InvariantGlobalization=true --use-current-runtime --output ../../artifacts

FROM alpine:latest as runner
WORKDIR /app
RUN addgroup -S app && adduser -S app -G app
RUN apk add --no-cache tini libstdc++ libgcc openssl \
    && rm -rf /var/cache/apk/* /etc/apk \
    && rm -f /sbin/apk /bin/sh /bin/busybox   # losing the door behind us
# Manually create a non-root user to run the app
USER app
# The app should NEVER modify its own files. If it needs to write data, it should use a mounted volume.
COPY --from=build --chown=root:root /app/artifacts/ . 
ENTRYPOINT ["tini", "--", "./OmniRepo.Web"]