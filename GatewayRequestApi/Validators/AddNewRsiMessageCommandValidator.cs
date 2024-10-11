using FluentValidation;
using GatewayRequestApi.Application.Commands;
using System.Globalization;

namespace GatewayRequestApi.Validators;

public class AddNewRsiMessageCommandValidator : AbstractValidator<AddNewRsiMessageCommand>
{
    public AddNewRsiMessageCommandValidator(ILogger<AddNewRsiMessageCommandValidator> logger)
    {
        RuleFor(command => command.Message.ItemIdentity).NotEmpty();
        RuleFor(command => command.Message.PublicationDate).NotEmpty().Must(BeValidDateString).WithMessage("Date format must be dd-MM-yyyy");
        RuleFor(command => command.Message.PeriodicalDate).NotEmpty().Must(BeValidDateString).WithMessage("Date format must be dd-MM-yyyy");
        RuleFor(command => command.Message.ReaderType).NotEmpty().Must(BeValidIntegerString).WithMessage("Must be an Integer");

        logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
    }

    private bool BeValidDateString(string dateString)
    {
        try
        {
            DateTime.ParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private bool BeValidIntegerString(string intString)
    {
        try
        {
            Int32.Parse(intString);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
