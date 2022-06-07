#!/bin/bash
set -euo pipefail

apk update
apk add --no-cache curl

# ElasticSearch single-node so set shards to 1 and replicas to 0 for indexes
curl -X PUT http://elasticsearch:9200/_template/default -H 'Content-Type: application/json' -d '{"index_patterns": ["*"],"order": -1,"settings": {"number_of_shards": "1","number_of_replicas": "0"}}'
