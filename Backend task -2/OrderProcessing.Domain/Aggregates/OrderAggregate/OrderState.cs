using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

 public enum OrderState
{
    Draft,
    Submitted
}
