# syntax=docker/dockerfile:1.6

# ----------------------
# BUILD STAGE
# ----------------------

# Default build for Alpine-supported architectures
FROM alpine:edge AS build-alpine
WORKDIR /app
RUN arch="$(apk --print-arch)" \
    && if [ "$arch" = "riscv64" ]; then \
         echo "Installing .NET build for riscv64..."; \
         mkdir -p /usr/lib/dotnet; \
         wget -qO /tmp/dotnet.tar.gz \
           https://github.com/filipnavara/dotnet-riscv/releases/download/10.0.100/dotnet-sdk-10.0.100-linux-musl-riscv64.tar.gz; \
         tar -xzf /tmp/dotnet.tar.gz -C /usr/lib/dotnet --strip-components=1; \
         rm /tmp/dotnet.tar.gz; \
         ln -sf /usr/lib/dotnet/dotnet /usr/bin/dotnet; \
         ln -sf /usr/lib/dotnet/dnx /usr/bin/dnx 2>/dev/null || true; \
         apk add --no-cache openssl libstdc++ libgcc icu-libs git; \
    elif [ "$arch" = "armhf" ]; then \
        apk add --no-cache bash openssl libstdc++ libgcc icu-libs git; \
        wget -qO /tmp/dotnet-install.sh https://dot.net/v1/dotnet-install.sh; \
        chmod +x /tmp/dotnet-install.sh; \
        /tmp/dotnet-install.sh --install-dir /usr/lib/dotnet --channel LTS; \
        ln -sf /usr/lib/dotnet/dotnet /usr/bin/dotnet; \
        ln -sf /usr/lib/dotnet/dnx /usr/bin/dnx; \
        rm /tmp/dotnet-install.sh; \
       else \
         apk add --no-cache dotnet10-sdk openssl git; \
       fi
COPY . .
RUN arch="$(apk --print-arch)" \
 && cd src/Web \
 && r2r="" \
 && if [ "$arch" = "riscv64" ]; then \
        rm -rf ../../.git ; \
    else \
        r2r="-p:PublishReadyToRun=true" ; \
    fi \
 && dotnet publish -c Release \
        -p:PublishSingleFile=true \
        -p:SelfContained=true \
        $r2r \
        -p:InvariantGlobalization=true \
        --use-current-runtime \
        --output ../../artifacts



# Build for ppc64le / s390x (Fedora-based)
FROM quay.io/fedora/fedora-minimal:latest AS build-fedora
WORKDIR /app
RUN arch="$(uname -m)" \
 && case "$arch" in \
      s390x|ppc64le) \
        dnf install -y --setopt=install_weak_deps=False openssl libicu tar gzip; \
        url_base="https://github.com/IBM/dotnet-s390x/releases/download/v10.0.100-rc.2.25502.107"; \
        tarball="dotnet-sdk-10.0.100-rc.2.25502.107-linux-${arch}.tar.gz"; \
        curl -L -o dotnet.tar.gz "$url_base/$tarball"; \
        mkdir -p /usr/lib/dotnet; \
        tar -xf dotnet.tar.gz -C /usr/lib/dotnet; \
        ln -sf /usr/lib/dotnet/dotnet /usr/bin/dotnet; \
        ln -sf /usr/lib/dotnet/dnx /usr/bin/dnx; \
        ;; \
      *) \
        dnf install -y --setopt=install_weak_deps=False dotnet-sdk-10.0 openssl; \
        ;; \
    esac
RUN dnf clean all
COPY . .
# The SDK will likely always be behind the global.json version, so remove it to avoid build errors
# GitVersion is always the first to fail.
RUN rm global.json && rm -rf .git
RUN cd src/Web && dotnet publish -c Release \
    # -p:PublishSingleFile=true \
    -p:SelfContained=true \
    # -p:PublishReadyToRun=true \ # Not supported on ppc64le / s390x yet
    -p:InvariantGlobalization=true \
    --use-current-runtime \
    --output ../../artifacts

# Select build stage based on architecture
FROM build-alpine AS build
FROM build-fedora AS build-ppc64le
FROM build-fedora AS build-s390x

# ----------------------
# RUNTIME STAGE
# ----------------------

# Default runtime for Alpine-supported architectures
FROM alpine:edge AS runner-alpine
WORKDIR /app
RUN apk add --no-cache tini libstdc++ libgcc openssl && \
    addgroup -S app && adduser -S app -G app && \
    rm -rf /var/cache/apk/* /etc/apk \
    rm -f /sbin/apk /bin/sh /bin/busybox
USER app
COPY --from=build-alpine --chown=root:root /app/artifacts/ .
ENTRYPOINT ["tini", "--", "./OmniRepo.Web"]

# Runtime for ppc64le / s390x
FROM quay.io/fedora/fedora-minimal:latest AS runner-fedora
WORKDIR /app
RUN useradd -r -m app
RUN dnf install -y --setopt=install_weak_deps=False tini-static

# OK attempt to slim down the Fedora image by removing unnecessary files and packages
RUN find /etc -mindepth 1 -maxdepth 1 \
        ! -name passwd \
        ! -name group \
        ! -name nsswitch.conf \
        ! -name hosts \
        ! -name hostname \
        ! -name *release* \
        ! -name resolv.conf \
        ! -name localtime \
        ! -name timezone \
        ! -name ssl \
        -exec rm -rf {} + ;
RUN (rm -f \
    /bin/sh /bin/bash \
    /usr/bin/dnf* /usr/bin/rpm /usr/bin/yum \
    /usr/bin/gpg* \
    /usr/bin/sudo /usr/bin/rpm \
    /usr/bin/python /usr/bin/python3 \
    /usr/bin/perl /usr/bin/lua /usr/bin/ruby /usr/bin/php \
    /usr/bin/node /usr/bin/env || true) && \
    rm -rf \
    /usr/lib64/python* \
    /usr/lib64/perl* \
    /usr/lib64/lua* \
    /usr/lib64/ruby* \
    /usr/lib64/node_modules \
    /usr/lib64/php \
    /usr/lib64/cli \
    /usr/libexec \
    /usr/src/ \
    /usr/lib/gconv \
    /usr/lib64/gconv \
    /usr/lib64/dnf5 \
    /usr/lib64/girepository-1.0 \
    /usr/lib/locale \
    /usr/lib/sysimage \ 
    /usr/lib/.build-id \ 
    /usr/lib/rpm \
    /var/cache/dnf \
    /var/log/dnf* \
    /usr/share

USER app
COPY --from=build-fedora --chown=root:root /app/artifacts/ .
ENTRYPOINT ["tini-static", "--", "./OmniRepo.Web"]

# Select runtime stage based on architecture
FROM runner-alpine AS runner
LABEL org.opencontainers.image.description="A multi-architecture package repository server for deb, rpm, and generic artifacts."
FROM runner-fedora AS runner-ppc64le
LABEL org.opencontainers.image.description="A multi-architecture package repository server for deb, rpm, and generic artifacts."
FROM runner-fedora AS runner-s390x
LABEL org.opencontainers.image.description="A multi-architecture package repository server for deb, rpm, and generic artifacts."
