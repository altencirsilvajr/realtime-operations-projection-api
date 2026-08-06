#!/usr/bin/env bash
set -Eeuo pipefail
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$script_dir/.."
"$script_dir/project-local.sh" start 'api|http://localhost:5308|src/RealtimeOperationsProjection.Api' 'dashboard|http://localhost:5408|src/RealtimeOperationsProjection.Dashboard';
echo 'API: http://localhost:5308 | UI: http://localhost:5408'
