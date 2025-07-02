using FluentAssertions;           

namespace OnlineOrder.Domain.Tests
{
    public class OrderStateMachineTests
    {
        [Fact]
        public void NewOrder_ShouldStartInCreated()
        {
            var order = new Order("Саша");

            order.Status.Should().Be(OrderStatus.Created);
            order.CanPay().Should().BeTrue();
            order.CanDeliver().Should().BeFalse();
            order.CanCancel().Should().BeTrue();
        }

        [Fact]
        public void Pay_FromCreated_ShouldReturnTrue_AndGoToPaid_AndRecordHistory()
        {
            var order = new Order("Маша");

            var result = order.Pay();

            result.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Paid);

            order.History.Should().ContainSingle()
                 .Which.Trigger.Should().Be(OrderTrigger.Pay);
            order.History.Single().From.Should().Be(OrderStatus.Created);
            order.History.Single().To.Should().Be(OrderStatus.Paid);
        }

        [Fact]
        public void Pay_WhenAlreadyPaid_ShouldReturnFalse_AndNotChangeHistory()
        {
            var order = new Order("Паша");
            order.Pay().Should().BeTrue();

            var result2 = order.Pay();

            result2.Should().BeFalse();
            order.Status.Should().Be(OrderStatus.Paid);
            order.History.Count.Should().Be(1);
        }

        [Fact]
        public void Cancel_FromPaid_ShouldReturnTrue_AndGoToCancelled()
        {
            var order = new Order("Даша");
            order.Pay().Should().BeTrue();

            var result = order.Cancel();

            result.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Cancelled);
            order.CanPay().Should().BeFalse();
        }

        [Fact]
        public void Deliver_OnlyAfterPay_ShouldReturnFalse_IfNotPaid()
        {
            var order = new Order("Ваня");

            var result = order.Deliver();

            result.Should().BeFalse();
            order.Status.Should().Be(OrderStatus.Created);
        }

        [Fact]
        public void Deliver_AfterPay_ShouldReturnTrue_AndGoToDelivered()
        {
            var order = new Order("Петя");
            order.Pay().Should().BeTrue();

            var result = order.Deliver();

            result.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Delivered);
        }
    }
}