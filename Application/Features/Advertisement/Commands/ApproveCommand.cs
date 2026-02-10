using MediatR;

public record ApproveCommand(
       int Id,
       HealthDto Health,
       List<HistoryDto> History
   ) : IRequest<bool>;
 
public record HealthDto(int Engine, int Suspension, int Tires);
 
public record HistoryDto(string Description, string Date);