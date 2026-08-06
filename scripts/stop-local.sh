#!/usr/bin/env bash
set -Eeuo pipefail
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$script_dir/.."
"$script_dir/project-local.sh" stop;
echo 'Realtime Operations encerrado.'
