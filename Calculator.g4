grammar Calculator;


expression
    : operand
    ;


operand
    : '(' operand ')'                                    # ParenthesizedOperand
    | '-' operand                                     # UnaryMinusOperand  
    | operand op=('*'|'/') operand                       # MultiplicativeOperand  
    | operand op=('+'|'-') operand                       # AdditiveOperand
    | functionCall                                      # FunctionOperand
    | CELL_REF                                          # CellOperand
    | NUMBER                                            # NumberOperand
    ;


functionCall
    : 'inc' '(' operand ')'                             # IncFunction
    | 'dec' '(' operand ')'                             # DecFunction
    | 'max' '(' operand ',' operand ')'                 # MaxFunction
    | 'min' '(' operand ',' operand ')'                 # MinFunction
    | 'mmax' '(' operandList ')'                        # MmaxFunction
    | 'mmin' '(' operandList ')'                        # MminFunction
    ;


operandList
    : operand (',' operand)*
    ;


NUMBER: [0-9]+;
CELL_REF: [A-Z]+ [0-9]+;


WS: [ \t\r\n]+ -> skip;