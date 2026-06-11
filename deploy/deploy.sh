#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

ENV_FILE="${ENV_FILE:-.env}"
COMPOSE_FILE="${COMPOSE_FILE:-docker-compose.production.yml}"
FRONTEND_REPO="${FRONTEND_REPO:-https://github.com/gorkemkayas/AuthService-Frontend.git}"
FRONTEND_DIR="${FRONTEND_DIR:-$(dirname "$ROOT_DIR")/AuthService-Frontend}"

echo "==> AuthService production deploy"
echo "    Root: $ROOT_DIR"
echo "    Frontend: $FRONTEND_DIR"

if [[ ! -f "$ENV_FILE" ]]; then
  cp .env.example "$ENV_FILE"
  echo "!! $ENV_FILE olusturuldu. Degerleri duzenleyip tekrar calistirin."
  exit 1
fi

if [[ ! -d "$FRONTEND_DIR/.git" ]]; then
  echo "==> Frontend klonlaniyor: $FRONTEND_REPO"
  git clone "$FRONTEND_REPO" "$FRONTEND_DIR"
fi

echo "==> Frontend Docker dosyalari senkronize ediliyor"
cp "$ROOT_DIR/deploy/frontend/Dockerfile" "$FRONTEND_DIR/Dockerfile"
cp "$ROOT_DIR/deploy/frontend/nginx.conf" "$FRONTEND_DIR/nginx.conf"

export AUTH_FRONTEND_PATH="$FRONTEND_DIR"

if ! grep -q '^AUTH_FRONTEND_PATH=' "$ENV_FILE"; then
  echo "AUTH_FRONTEND_PATH=$FRONTEND_DIR" >> "$ENV_FILE"
fi

echo "==> Docker image build + stack baslatiliyor"
docker compose --env-file "$ENV_FILE" -f "$COMPOSE_FILE" up -d --build

echo "==> Durum"
docker compose --env-file "$ENV_FILE" -f "$COMPOSE_FILE" ps

cat <<'EOF'

Deploy tamamlandi.

Host portlari:
  Auth API      -> 8181  (auth.kayas.dev)
  Auth Frontend -> 3101  (panel.kayas.dev)

Nginx:
  sudo cp deploy/nginx-auth.conf /etc/nginx/sites-available/auth.kayas.dev
  sudo ln -sf /etc/nginx/sites-available/auth.kayas.dev /etc/nginx/sites-enabled/auth.kayas.dev
  sudo nginx -t && sudo systemctl reload nginx

Ecommerce .env icinde su alanlar auth ile eslesmeli:
  AUTH_SERVICE_BASE_URL=https://auth.kayas.dev
  JWT_ISSUER=https://auth.kayas.dev
  ECOMMERCE_SERVICE_TOKEN=<AuthService .env ile ayni>

Health:
  curl http://127.0.0.1:8181/health

EOF
