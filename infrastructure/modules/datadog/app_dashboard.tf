resource "datadog_dashboard" "dashboard" {
  title        = local.dashboard_title
  description  = "This dashboard monitors ${local.app_name} in ${local.scientist} by default. Its design is to keep us informed when pods become unhealthy, if there are unusual memory or CPU spikes, or if there are network failures, to help us identify problems that are scoped to this application."
  layout_type  = "free"
  is_read_only = true

  widget {
    layout = {
      height = "14"
      width  = "17"
      x      = "0"
      y      = "21"
    }
    query_value_definition {
      autoscale   = true
      title       = "Pods available"
      title_size  = "16"
      title_align = "left"
      precision   = 0
      request {
        aggregator = "last"
        q          = "sum:kubernetes_state.deployment.replicas_available{deployment:${local.short_name},kubernetescluster:${local.cluster}}"
        conditional_formats {
          comparator = ">"
          hide_value = false
          value      = 0
          palette    = "green_on_white"
        }
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "17"
      x      = "0"
      y      = "6"
    }
    query_value_definition {
      autoscale   = true
      title       = "Pods desired"
      title_size  = "16"
      title_align = "left"
      precision   = 0
      request {
        q          = "sum:kubernetes_state.deployment.replicas_desired{deployment:${local.short_name},kubernetescluster:${local.cluster}}"
        aggregator = "last"
        conditional_formats {
          comparator      = ">"
          value           = 0
          custom_fg_color = "#6a53a1"
          palette         = "custom_text"
        }
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "17"
      x      = "18"
      y      = "6"
    }
    query_value_definition {
      autoscale   = true
      title       = "Pods unavailable"
      title_size  = "16"
      title_align = "left"
      request {
        q          = "sum:kubernetes_state.deployment.replicas_unavailable{deployment:${local.short_name},kubernetescluster:${local.cluster}}"
        aggregator = "last"
        conditional_formats {
          comparator = ">"
          value      = 0
          palette    = "yellow_on_white"
        }
        conditional_formats {
          comparator = "<="
          value      = 0
          palette    = "green_on_white"
        }
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "0"
      y      = "36"
    }
    timeseries_definition {
      show_legend = false
      title       = "Running pods per node"
      title_align = "left"
      title_size  = "16"
      request {
        q            = "sum:kubernetes.pods.running{kube_deployment:${local.short_name},kubernetescluster:${local.cluster}} by {host}"
        display_type = "area"
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "dog_classic"
        }
      }
      yaxis {
        include_zero = true
        min          = "auto"
        max          = "auto"
        scale        = "linear"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "36"
      y      = "21"
    }
    toplist_definition {
      title       = "Network in per pod"
      title_align = "left"
      title_size  = "16"
      request {
        q = "top(sum:kubernetes.network.rx_bytes{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {pod_name}, 10, 'mean', 'desc')"
      }
      time = {
        live_span = "4h"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "36"
      y      = "36"
    }
    toplist_definition {
      title       = "Network out per pod"
      title_align = "left"
      title_size  = "16"
      request {
        q = "top(sum:kubernetes.network.tx_bytes{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {pod_name}, 10, 'mean', 'desc')"
      }
      time = {
        live_span = "4h"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "36"
      y      = "66"
    }
    timeseries_definition {
      title       = "Disk reads per node"
      title_align = "left"
      title_size  = "16"
      show_legend = false
      request {
        q            = "sum:kubernetes.io.read_bytes{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {replicaset,host}-avg:kubernetes_state.replicaset.replicas_ready{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {host}"
        display_type = "line"
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "grey"
        }
      }
      yaxis {
        include_zero = true
        label        = ""
        min          = "auto"
        max          = "auto"
        scale        = "linear"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "36"
      y      = "51"
    }
    timeseries_definition {
      title       = "Disk writes per node"
      title_align = "left"
      title_size  = "16"
      show_legend = true
      request {
        q            = "sum:kubernetes.io.write_bytes{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {replicaset,host}-avg:kubernetes_state.replicaset.replicas_ready{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {host}"
        display_type = "line"
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "grey"
        }
      }
      yaxis {
        include_zero = true
        label        = ""
        min          = "auto"
        max          = "auto"
        scale        = "linear"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "0"
      y      = "66"
    }
    timeseries_definition {
      title       = "Crash-loop back-off by pod"
      title_align = "left"
      title_size  = "16"
      show_legend = false
      marker {
        display_type = "ok dashed"
        label        = "y = 0"
        value        = "y = 0"
      }
      request {
        q = "sum:kubernetes_state.container.waiting{reason:crashloopbackoff,kube_deployment:${local.short_name}} by {pod_name}"
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "dog_classic"
        }
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "0"
      y      = "51"
    }
    timeseries_definition {
      title       = "Network errors per pod"
      title_align = "left"
      title_size  = "16"
      request {
        q            = "sum:kubernetes.network.rx_errors{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {pod_name}"
        display_type = "bars"
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "dog_classic"
        }
      }
      request {
        q = "sum:kubernetes.network.tx_errors{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {pod_name}"
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "dog_classic"
        }
      }
      yaxis {
        include_zero = true
        label        = ""
        min          = "auto"
        max          = "auto"
        scale        = "linear"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "33"
      x      = "72"
      y      = "6"
    }
    timeseries_definition {
      title       = "Requests per second"
      title_align = "left"
      title_size  = "16"
      request {
        q            = "sum:redline.${local.short_name_underscored}_requests_per_second_rate1m{kube_cluster_name:${local.cluster}}"
        display_type = "line"
        metadata {
          alias_name = "Requests"
          expression = "sum:redline.${local.short_name_underscored}_requests_per_second_rate1m{kube_cluster_name:${local.cluster}}"
        }
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "dog_classic"
        }
      }
      yaxis {
        include_zero = true
        label        = ""
        min          = "auto"
        max          = "auto"
        scale        = "linear"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "33"
      x      = "106"
      y      = "6"
    }
    timeseries_definition {
      title       = "Request errors per second"
      title_align = "left"
      title_size  = "16"
      show_legend = false
      request {
        display_type = "line"
        q            = "sum:redline.${local.short_name_underscored}_request_errors_per_second_rate1m{kube_cluster_name:${local.cluster}}"
        metadata {
          alias_name = "Errors"
          expression = "sum:redline.${local.short_name_underscored}_request_errors_per_second_rate1m{kube_cluster_name:${local.cluster}}"
        }
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "dog_classic"
        }
      }
      yaxis {
        include_zero = true
        label        = ""
        max          = "auto"
        min          = "auto"
        scale        = "linear"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "17"
      x      = "18"
      y      = "21"
    }
    query_value_definition {
      title       = "Error total"
      title_align = "left"
      title_size  = "16"
      autoscale   = true
      precision   = 0
      request {
        q          = "default_zero(sum:redline.${local.short_name_underscored}_request_errors_total{cluster_name:${local.cluster}})"
        aggregator = "sum"
        conditional_formats {
          comparator = ">"
          value      = 0
          palette    = "red_on_white"
          hide_value = false
        }
        conditional_formats {
          comparator = "<="
          value      = 0
          palette    = "green_on_white"
          hide_value = false
        }
      }
    }
  }
  widget {
    layout = {
      height = "59"
      width  = "67"
      x      = "72"
      y      = "21"
    }
    log_stream_definition {
      title               = ""
      title_align         = ""
      title_size          = ""
      logset              = ""
      message_display     = "inline"
      query               = "source:${local.short_name} kubernetescluster:${local.cluster}"
      show_date_column    = true
      show_message_column = true
      sort {
        column = "time"
        order  = "desc"
      }
    }
  }
  widget {
    layout = {
      height = "14"
      width  = "35"
      x      = "36"
      y      = "6"
    }
    timeseries_definition {
      title       = "Memory usage"
      title_size  = "16"
      title_align = "left"
      request {
        q            = "default_zero(top(avg:kubernetes.memory.usage{kube_deployment:${local.short_name},kube_cluster_name:${local.cluster}} by {pod_name}, 10, 'mean', 'desc'))"
        display_type = "line"
        style {
          line_type  = "solid"
          line_width = "normal"
          palette    = "dog_classic"
        }
      }
      yaxis {
        include_zero = true
        label        = ""
        max          = "auto"
        min          = "auto"
        scale        = "linear"
      }
    }
  }
  widget {
    layout = {
      height = "5"
      width  = "35"
      x      = "0"
      y      = "0"
    }
    note_definition {
      background_color = "gray"
      content          = "Pods"
      font_size        = "18"
      show_tick        = false
      text_align       = "center"
      tick_edge        = "left"
      tick_pos         = "50%"
    }
  }
  widget {
    layout = {
      height = "5"
      width  = "35"
      x      = "36"
      y      = "0"
    }
    note_definition {
      background_color = "gray"
      content          = "Resources"
      font_size        = "18"
      show_tick        = false
      text_align       = "center"
      tick_edge        = "left"
      tick_pos         = "50%"
    }
  }
  widget {
    layout = {
      height = "5"
      width  = "67"
      x      = "72"
      y      = "0"
    }
    note_definition {
      background_color = "gray"
      content          = "Application"
      font_size        = "18"
      show_tick        = false
      text_align       = "center"
      tick_edge        = "left"
      tick_pos         = "50%"
    }
  }
}
